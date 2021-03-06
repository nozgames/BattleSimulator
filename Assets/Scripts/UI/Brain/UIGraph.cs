using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BattleSimulator.UI;
using BattleSimulator.AI;
using System;

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
            CreateNode
        }

        [Header("General")]
        [SerializeField] private RectTransform _zoomTransform = null;
        [SerializeField] private UIGrid _grid = null;
        [SerializeField] private RectTransform _nodeTransform = null;
        [SerializeField] private RectTransform _wires = null;
        [SerializeField] private RectTransform _dragNodes = null;
        [SerializeField] private UIWireRenderer _dragWire = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject _nodePrefab = null;
        [SerializeField] private GameObject _compressedNodePrefab = null;
        [SerializeField] private GameObject _wirePrefab = null;

        private List<UINode> _selected = new List<UINode>();
        private int _zoomLevel = 10;
        private Drag _drag = Drag.None;
        private UIPort _dragPort = null;
        private UINode _dragNode = null;
        private Vector2 _dragStart;
        private Vector2 _dragAnchorStart;
        

        private List<UINode> _nodes = new List<UINode>();

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

        private void Start()
        {
            zoomLevel = 10;

            CreateNode(new AI.FloatToPriority(), Vector2.zero);
            CreateNode(new AI.DistanceNode(), Vector2.zero);
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
            foreach (var hover in eventData.hovered)
            {
                var component = hover.GetComponent<T>();
                if (null != component)
                    return component;
            }

            return null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragStart = eventData.position;            
            _dragPort = GetHoverComponent<UIPort>(eventData);
            _drag = Drag.None;            

            if (null == _dragPort)
                _dragNode = GetHoverComponent<UINode>(eventData);

            // Make sure the node that is clicked on is selected, but dont clear the other selection 
            // until we know there is no drag operation going on
            if (_drag == Drag.None && eventData.button == PointerEventData.InputButton.Left)
                if (_dragPort != null)
                    SelectNode(_dragPort.node);
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
                        var from = _dragPort.port.flow == PortFlow.Output ? _dragPort : targetPort;
                        var to = _dragPort.port.flow == PortFlow.Input ? _dragPort : targetPort;
                        UIWire.Create(
                            new Wire(from.port, to.port),
                            _wirePrefab,
                            _wires,
                            from,
                            to);
                    }

                    break;
                }

                case Drag.None:
                    if (_dragNode != null && !Input.GetKey(KeyCode.LeftShift))
                        SelectNode(_dragNode);
                    break;
            }

            _dragWire.gameObject.SetActive(false);
            _drag = Drag.None;
            _dragPort = null;
            _dragNode = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_dragPort != null)
                {
                    var portBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_zoomTransform, _dragPort.connection.GetComponent<RectTransform>());

                    _dragStart = _zoomTransform.InverseTransformPoint(_dragStart);
                    _dragWire.fromColor = UIManager.GetPortColor(_dragPort.port);
                    _dragWire.toColor = UIManager.GetPortColor(_dragPort.port);
                    _dragWire.from = portBounds.center;
                    _dragWire.to = portBounds.center;
                    _dragWire.gameObject.SetActive(true);
                    _dragAnchorStart = portBounds.center;
                    _drag = Drag.Port;
                    return;
                }

                if (_dragNode != null)
                {
                    _dragAnchorStart = _dragNode.GetComponent<RectTransform>().anchoredPosition;
                    _drag = Drag.Node;
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

        public void OnDrag(PointerEventData eventData)
        {
            switch (_drag)
            {
                case Drag.Node:
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_dragNode.transform.parent, eventData.position - eventData.delta, eventData.pressEventCamera, out var pt0);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_dragNode.transform.parent, eventData.position, eventData.pressEventCamera, out var pt1);
                    var delta = pt1 - pt0;

                    if (_dragNode.selected)
                    {
                        foreach (var selected in _selected)
                            selected.Move(delta);
                    }
                    else
                        _dragNode.MoveTo(delta);
                    
                    break;

                case Drag.Port:
                    
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    var position = (Vector2)_zoomTransform.InverseTransformPoint(eventData.position);
                    if (_dragPort.port.flow == PortFlow.Input)
                        _dragWire.from = _dragAnchorStart + (position - _dragStart);
                    else
                        _dragWire.to = _dragAnchorStart + (position - _dragStart);

                    // Snap to valid ports
                    // TODO: change cursor when port is invalid like blueprint does
                    var snapPort = GetHoverComponent<UIPort>(eventData);
                    if(_dragPort.CanConnectTo(snapPort))
                    {
                        var snapPortCenter = RectTransformUtility.CalculateRelativeRectTransformBounds(_zoomTransform, snapPort.connection.GetComponent<RectTransform>()).center;
                        if (snapPort.port.flow == PortFlow.Input)
                            _dragWire.to = snapPortCenter;
                        else
                            _dragWire.from = snapPortCenter;
                    }

                    break;

                case Drag.Graph:
                    if (eventData.button != PointerEventData.InputButton.Right)
                        return;
                    
                    _zoomTransform.anchoredPosition = _dragAnchorStart + (eventData.position - _dragStart);
                    _grid.GridOffset = _zoomTransform.anchoredPosition;
                    break;
            }
        }

        public UINode CreateNode (Node node, Vector2 position, RectTransform parent = null)
        {
            var nodeInfo = NodeInfo.Create(node);
            if (null == nodeInfo)
                return null;

            GameObject prefab = null;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compressed))
                prefab = _compressedNodePrefab;
            else
                prefab = _nodePrefab;

            var uinode = UINode.Create(node, prefab, parent == null ? _nodeTransform : parent, position);
            _nodes.Add(uinode);
            return uinode;
        }

        public void BeginDrag (AI.NodeInfo nodeInfo, PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_zoomTransform, eventData.position, eventData.pressEventCamera, out var position);

            var node = (AI.Node)Activator.CreateInstance(nodeInfo.nodeType);
            _dragNode = CreateNode(node, position, _dragNodes);
            _dragStart = position;
            _dragAnchorStart = position;
            _drag = Drag.CreateNode;
            SelectNode(_dragNode);
        }

        public void EndDrag (AI.NodeInfo nodeInfo, PointerEventData eventData)
        {
            if (_drag != Drag.CreateNode)
                return;

            _dragNode.transform.SetParent(_nodeTransform);
        }

        public void ContinueDrag(AI.NodeInfo nodeInfo, PointerEventData eventData)
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
    }
}
