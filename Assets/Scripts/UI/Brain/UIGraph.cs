using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BattleSimulator.UI;
using BattleSimulator.Simulation;

namespace BattleSimulator
{
    public class UIGraph : MonoBehaviour, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private enum Drag
        {
            None,
            Graph,
            Port,
            Node,
            Wire,
            CreateNode
        }

        [Header("General")]
        [SerializeField] private RectTransform _zoomTransform = null;
        [SerializeField] private UIGrid _grid = null;
        [SerializeField] private RectTransform _nodeTransform = null;
        [SerializeField] private RectTransform _wires = null;
        [SerializeField] private RectTransform _dragNodes = null;
        [SerializeField] private UIWireRenderer _dragWireRenderer = null;
        [SerializeField] private RectTransform _trash = null;
        [SerializeField] private UINodePalette _nodePalette = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject _nodePrefab = null;
        [SerializeField] private GameObject _compressedNodePrefab = null;
        [SerializeField] private GameObject _wirePrefab = null;
        [SerializeField] private GameObject[] _nodePrefabs = null;

        private UnitDef _unitDef;
        private List<UINode> _selected = new List<UINode>();
        private int _zoomLevel = 10;
        private Drag _drag = Drag.None;
        private UIPort _dragPort = null;
        private UINode _dragNode = null;
        private Vector2 _dragStart;
        private Vector2 _dragAnchorStart;
        private UIWire _dragWire = null;
        private Stack<Command> _undo = new Stack<Command>();
        private Stack<Command> _redo = new Stack<Command>();

        private List<UINode> _nodes;

        public RectTransform trash => _trash;

        public List<UINode> nodes => _nodes;

        private int zoomLevel 
        {
            get => _zoomLevel;
            set {
                _zoomLevel = Mathf.Clamp(value, 1, 25);
                _dragNodes.localScale = _zoomTransform.localScale = Vector3.one * Mathf.Pow(1.2f,zoomLevel-10);                
                _grid.GridScale = _zoomTransform.localScale.x;
                _grid.color = new Color(_grid.color.r, _grid.color.g, _grid.color.b, (_zoomLevel / 25.0f) * 0.6f);
            }
        }

        public static UIGraph Create (Graph graph, GameObject prefab, RectTransform parent)
        {
            var uigraph = Instantiate(prefab, parent).GetComponent<UIGraph>();

            // Load the unit definition for the given graph
            uigraph._unitDef = GameSystem.unitDatabase.GetRecord<UnitDef>(graph.unitDef);
            if (uigraph._unitDef != null)
            {
                foreach (var ability in uigraph._unitDef.abilities)
                    uigraph._nodePalette.Add(ability);
            }

            uigraph._nodes = new List<UINode>(graph.nodes.Count);
            foreach (var node in graph.nodes)
            {
                if (node is AbilityNodeWithTarget abilityNode)
                {
                    var ability = uigraph._unitDef.GetAbility(abilityNode.guid);
                    if(ability != null)
                        uigraph.CreateNode(ability, node.position);
                } else
                    uigraph.CreateNode(NodeInfo.Create(node), node.position);
            }

            for(int nodeIndex=0; nodeIndex < graph.nodes.Count; nodeIndex++)
            {
                var uinode = uigraph._nodes[nodeIndex];
                var node = graph.nodes[nodeIndex];

                var uiNodeProperties = uinode.GetComponents<UINodeProperty>();
                foreach (var uiNodeProperty in uiNodeProperties)
                    uiNodeProperty.Write(node);

                foreach(var fromPortInfo in uinode.nodeInfo.ports)
                {
                    if (fromPortInfo.flow != PortFlow.Output)
                        continue;

                    var uiport = uinode.GetPort(fromPortInfo);

                    var fromPort = fromPortInfo.GetPort(node);
                    foreach (var wire in fromPort.wires)
                        uigraph.CreateWire(
                            uiport,
                            uigraph._nodes[graph.nodes.IndexOf(wire.to.node)].GetPort(wire.to.info));
                }
            }

            return uigraph;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
                Undo();

            if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
                Redo();

            if (Input.GetKeyDown(KeyCode.Delete))
                DeleteSelectNodes();

            UpdateCursor(Input.mousePosition);
        }

        private void Start()
        {
            zoomLevel = 10;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if(eventData.scrollDelta.y > 0)
                zoomLevel++;
            else if (eventData.scrollDelta.y < 0)
                zoomLevel--;
        }

        private T GetHoverComponent<T> (PointerEventData eventData) where T : MonoBehaviour
        {
            var results = new List<RaycastResult>();
            UIManager.RayCast(eventData.position, results);

            foreach (var result in results)
            {
                var component = result.gameObject.GetComponentInParent<T>();
                if (null != component)
                    return component;
            }

            return null;
        }

        private UIWire GetHoverWire (PointerEventData eventData) 
        {
            var results = new List<RaycastResult>();
            UIManager.RayCast(eventData.position, results);

            foreach (var result in results)
            {
                var uiwire = result.gameObject.GetComponent<UIWire>();
                if (null == uiwire)
                    continue;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(uiwire.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
                if (uiwire.HitTest(position))
                    return uiwire;
            }

            return null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragStart = eventData.position;
            _drag = Drag.None;

            _dragPort = GetHoverComponent<UIPort>(eventData);
            if(null == _dragPort)
            {
                _dragWire = GetHoverWire(eventData);
                if (_dragWire != null)
                    return;
            }

            if (null == _dragPort)
                _dragNode = GetHoverComponent<UINode>(eventData);

            // Make sure the node that is clicked on is selected, but dont clear the other selection 
            // until we know there is no drag operation going on
            if (_drag == Drag.None && eventData.button == PointerEventData.InputButton.Left)
                if (_dragPort != null)
                    SelectNode(_dragPort.uinode);
                else if (_dragNode != null && !_dragNode.selected)
                    SelectNode(_dragNode, Input.GetKey(KeyCode.LeftShift));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            switch (_drag)
            {
                case Drag.Port:
                {                    
                    var targetPort = GetHoverComponent<UIPort>(eventData);
                    if (_dragPort.CanConnectTo(targetPort))
                    {
                        var command = new GroupCommand();
                        var input = _dragPort.isInput ? _dragPort : targetPort;
                        if (!input.portInfo.flags.HasFlag(PortFlags.AllowMultipleWires) && input.wires.Count > 0)
                            command.Add(new DeleteWireCommand(input.wires[0]));

                        command.Add(new AddWireCommand(
                            _dragPort.portInfo.flow == PortFlow.Output ? _dragPort : targetPort,
                            _dragPort.portInfo.flow == PortFlow.Input ? _dragPort : targetPort));

                        Execute(command);
                    } 
                    // If dragging an exsiting wire and no connection was made then remove the connection
                    else if (targetPort == null && _dragWire != null)
                    {                        
                        Execute(new DeleteWireCommand(_dragWire));
                        _dragWire.gameObject.SetActive(true);
                    }

                    break;
                }

                case Drag.Node:
                {
                    
                    break;
                }

                case Drag.None:
                    if (_dragNode != null && !Input.GetKey(KeyCode.LeftShift))
                        SelectNode(_dragNode);
                    break;
            }

            _dragWireRenderer.gameObject.SetActive(false);
            _drag = Drag.None;
            _dragPort = null;
            _dragWire = null;
            _dragNode = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_dragWire != null)
                {
                    _dragWire.gameObject.SetActive(false);

                    // Which side of the wire was the cursor closer to?
                    var fromPosition = _dragWire.from.position;
                    var toPosition = _dragWire.to.position;

                    _dragStart = _zoomTransform.InverseTransformPoint(_dragStart);
                    if ((_dragStart - fromPosition).sqrMagnitude > (_dragStart - toPosition).sqrMagnitude)
                    {
                        _dragPort = _dragWire.from;
                        toPosition = _dragStart;
                    } 
                    else
                    {
                        _dragPort = _dragWire.to;
                        fromPosition = _dragStart;
                    }

                    _dragWireRenderer.fromColor = UIManager.GetPortColor(_dragPort.portInfo);
                    _dragWireRenderer.toColor = UIManager.GetPortColor(_dragPort.portInfo);
                    _dragWireRenderer.from = fromPosition;
                    _dragWireRenderer.to = toPosition;
                    _dragWireRenderer.gameObject.SetActive(true);
                    _dragAnchorStart = _dragStart;
                    _drag = Drag.Port;
                    return;
                }

                if (_dragPort != null)
                {
                    var portBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_zoomTransform, _dragPort.connection.GetComponent<RectTransform>());

                    _dragStart = _zoomTransform.InverseTransformPoint(_dragStart);
                    _dragWireRenderer.fromColor = UIManager.GetPortColor(_dragPort.portInfo);
                    _dragWireRenderer.toColor = UIManager.GetPortColor(_dragPort.portInfo);
                    _dragWireRenderer.from = portBounds.center;
                    _dragWireRenderer.to = portBounds.center;
                    _dragWireRenderer.gameObject.SetActive(true);
                    _dragAnchorStart = portBounds.center;
                    _drag = Drag.Port;
                    return;
                }

                if (_dragNode != null)
                {
                    _dragAnchorStart = _dragNode.GetComponent<RectTransform>().anchoredPosition;
                    _drag = Drag.Node;
                    MoveNodes(eventData, false);
                    return;
                }
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _drag = Drag.Graph;
                _dragAnchorStart = _zoomTransform.anchoredPosition;
                return;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }

        private void MoveNodes (PointerEventData eventData, bool merge = false)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_dragNode.transform.parent, eventData.position - eventData.delta, eventData.pressEventCamera, out var pt0);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_dragNode.transform.parent, eventData.position, eventData.pressEventCamera, out var pt1);
            var delta = pt1 - pt0;

            if (_dragNode.selected)
            {
                var group = new GroupCommand();
                foreach (var selected in _selected)
                    group.Add(new MoveNodeCommand(selected, selected.position + delta));
                Execute(group, merge);
            } else
                Execute(new MoveNodeCommand(_dragNode, _dragNode.position + delta), merge);
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (_drag)
            {
                case Drag.Node:
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    MoveNodes(eventData, true);
                    break;

                case Drag.Port:
                    
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    var position = (Vector2)_zoomTransform.InverseTransformPoint(eventData.position);
                    if (_dragPort.portInfo.flow == PortFlow.Input)
                        _dragWireRenderer.from = _dragAnchorStart + (position - _dragStart);
                    else
                        _dragWireRenderer.to = _dragAnchorStart + (position - _dragStart);

                    // Snap to valid ports
                    // TODO: change cursor when port is invalid like blueprint does
                    var snapPort = GetHoverComponent<UIPort>(eventData);
                    if(_dragPort.CanConnectTo(snapPort))
                    {
                        if (snapPort.portInfo.flow == PortFlow.Input)
                            _dragWireRenderer.to = snapPort.position;
                        else
                            _dragWireRenderer.from = snapPort.position;
                    }

                    break;

                case Drag.Graph:
                    if (eventData.button != PointerEventData.InputButton.Right)
                        return;

                    _dragNodes.anchoredPosition = _zoomTransform.anchoredPosition = _dragAnchorStart + (eventData.position - _dragStart);                    
                    _grid.GridOffset = _zoomTransform.anchoredPosition;
                    break;
            }
        }

        public UINode CreateNode(Abilities.Ability ability, Vector2 position, RectTransform parent = null)
        {
            var nodeInfo = NodeInfo.Create(typeof(AbilityNodeWithTarget));
            var prefab = _nodePrefab;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compact))
                prefab = _compressedNodePrefab;

            if (!string.IsNullOrEmpty(nodeInfo.prefab))
                foreach (var nodePrefab in _nodePrefabs)
                    if (string.Compare(nodePrefab.name, nodeInfo.prefab, false) == 0)
                    {
                        prefab = nodePrefab;
                        break;
                    }

            var uinode = UINode.Create(this, ability, prefab, parent == null ? _nodeTransform : parent, position);
            _nodes.Add(uinode);
            return uinode;
        }

        public UINode CreateNode (NodeInfo nodeInfo, Vector2 position, RectTransform parent = null)
        {
            var prefab = _nodePrefab;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compact))
                prefab = _compressedNodePrefab;

            if(!string.IsNullOrEmpty(nodeInfo.prefab))
                foreach(var nodePrefab in _nodePrefabs)
                    if(string.Compare(nodePrefab.name, nodeInfo.prefab, false) == 0)
                    {
                        prefab = nodePrefab;
                        break;
                    }

            var uinode = UINode.Create(this, nodeInfo, prefab, parent == null ? _nodeTransform : parent, position);
            _nodes.Add(uinode);
            return uinode;
        }

        public UIWire CreateWire (UIPort from, UIPort to) => UIWire.Create(from, to, _wirePrefab, _wires);

        public void DeleteSelectNodes()
        {
            var group = new GroupCommand();
            foreach (var selected in _selected)
            {
                foreach (var uiport in selected.ports)
                {
                    foreach (var wire in uiport.wires)
                    {
                        // To prevent double deleting the wire we check to see if input ports are 
                        // connected to a seleted node, if they are we assume they will be deleted by their outputs
                        if (uiport.portInfo.flow == PortFlow.Input && wire.from.uinode.selected)
                            continue;

                        group.Add(new DeleteWireCommand(wire));
                    }
                }

                group.Add(new DeleteNodeCommand(selected));
            }

            UnselectAllNodes();
            Execute(group);           
        }

        private void Execute (Command command, bool merge = true)
        {
            if(merge && _undo.Count > 0 && command.CanMerge(_undo.Peek()))
            { 
                _undo.Peek().Merge(command);
                return;
            }

            _undo.Push(command);

            // Clear the redo buffer.
            foreach (var redo in _redo)
                redo.Dispose();
            _redo.Clear();

            if(!command.isExecuted)
                command.OnExecute();

            command.selection = _selected.ToArray();
        }

        private void Undo ()
        {
            if (_undo.Count == 0)
                return;

            var command = _undo.Pop();
            _redo.Push(command);
            command.OnUndo();

            UnselectAllNodes();
            foreach (var selected in command.selection)
                SelectNode(selected, true);
        }

        private void Redo()
        {
            if (_redo.Count == 0)
                return;

            var command = _redo.Pop();
            _undo.Push(command);
            command.OnRedo();

            UnselectAllNodes();
            foreach (var selected in command.selection)
                SelectNode(selected, true);
        }

        public void BeginDrag (UINodePaletteItem item, PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_zoomTransform, eventData.position, eventData.pressEventCamera, out var position);

            _dragNode = item.ability != null ? 
                CreateNode(item.ability, position, _dragNodes) :
                CreateNode(item.nodeInfo, position, _dragNodes);
            _dragStart = position;
            _dragAnchorStart = position;
            _drag = Drag.CreateNode;
            SelectNode(_dragNode);
        }

        public void EndDrag (UINodePaletteItem item, PointerEventData eventData)
        {
            if (_drag != Drag.CreateNode)
                return;
            
            _dragNode.transform.SetParent(_nodeTransform);
            Execute(new AddNodeCommand(this, _dragNode, _dragNode.position));
        }

        public void ContinueDrag(UINodePaletteItem item, PointerEventData eventData)
        {
            if (_drag != Drag.CreateNode)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_zoomTransform, eventData.position, eventData.pressEventCamera, out var position);
            _dragNode.MoveTo(position);
        }

        public void SelectNode (UINode node, bool add = false)
        {
            if (!add)
                UnselectAllNodes();

            if (_selected.Contains(node))
                return;

            node.selected = true;
            _selected.Add(node);
        }

        public void UnselectNode(UINode node)
        {
            node.selected = false;
            _selected.Remove(node);
        }

        public void UnselectAllNodes ()
        {
            while (_selected.Count > 0)
                UnselectNode(_selected[0]);
        }        

        public BrainGraph ToGraph ()
        {
            var graph = new BrainGraph(_unitDef.guid);
            graph.nodes.Capacity = _nodes.Count;

            foreach (var uinode in _nodes)
            {
                var node = uinode.nodeInfo.CreateNode();
                if (null == node)
                    continue;

                if (node is AbilityNodeWithTarget abilityNode)
                    abilityNode.guid = uinode.ability.guid;

                graph.AddNode(node);
            }

            for(int nodeIndex=0; nodeIndex < _nodes.Count; nodeIndex++)
            {
                var uinode = _nodes[nodeIndex];
                var node = graph.nodes[nodeIndex];
                node.position = uinode.position;

                var uiNodeProperties = uinode.GetComponents<UINodeProperty>();
                foreach (var uiNodeProperty in uiNodeProperties)
                    uiNodeProperty.Read(node);

                foreach (var uiport in uinode.ports)
                {
                    if (uiport.portInfo.flow != PortFlow.Output)
                        continue;

                    foreach (var uiwire in uiport.wires)
                    {
                        var fromPort = uiport.portInfo.GetPort(node);
                        var toPort = uiwire.to.portInfo.GetPort(graph.nodes[_nodes.IndexOf(uiwire.to.uinode)]);
                        fromPort.ConnectTo(toPort);
                    }
                }
            }

            return graph;
        }

        private void UpdateCursor(Vector2 position)
        {

        }
    }
}
