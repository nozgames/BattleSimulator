using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BattleSimulator.UI;
using BattleSimulator.AI;
using System.Linq;

namespace BattleSimulator
{
    public class UIGraph : MonoBehaviour, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private enum Drag
        {
            None,
            Graph,
            Port,
            Node
        }

        [Header("General")]
        [SerializeField] private RectTransform _zoomTransform = null;
        [SerializeField] private UIGrid _grid = null;
        [SerializeField] private RectTransform _nodeTransform = null;
        [SerializeField] private UIWireRenderer _dragWire = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject _nodePrefab = null;
        [SerializeField] private GameObject _compressedNodePrefab = null;

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
                _zoomTransform.localScale = Vector3.one * Mathf.Pow(1.2f,zoomLevel-10);
                _grid.GridScale = _zoomTransform.localScale.x;
                _grid.color = new Color(_grid.color.r, _grid.color.g, _grid.color.b, (_zoomLevel / 25.0f) * 0.6f);
            }
        }

        private void Start()
        {
            zoomLevel = 10;

            CreateNode(new AI.FloatToPriority(), Vector2.zero);
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
        }

        public void OnPointerUp(PointerEventData eventData)
        {
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
            _dragWire.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (_drag)
            {
                case Drag.Node:
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    _dragNode.gameObject.GetComponent<RectTransform>().anchoredPosition = _dragAnchorStart + (eventData.position - _dragStart);
                    break;

                case Drag.Port:
                    
                    if (eventData.button != PointerEventData.InputButton.Left)
                        return;

                    var position = (Vector2)_zoomTransform.InverseTransformPoint(eventData.position);
                    if (_dragPort.port.flow == PortFlow.Input)
                        _dragWire.from = _dragAnchorStart + (position - _dragStart);
                    else
                        _dragWire.to = _dragAnchorStart + (position - _dragStart);
                    break;

                case Drag.Graph:
                    if (eventData.button != PointerEventData.InputButton.Right)
                        return;
                    
                    _zoomTransform.anchoredPosition = _dragAnchorStart + (eventData.position - _dragStart);
                    _grid.GridOffset = _zoomTransform.anchoredPosition;
                    break;
            }
        }

        public UINode CreateNode (Node node, Vector2 position)
        {
            var nodeInfo = NodeInfo.Create(node);
            if (null == nodeInfo)
                return null;

            GameObject prefab = null;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compressed))
                prefab = _compressedNodePrefab;
            else
                prefab = _nodePrefab;

            var uinode = UINode.Create(node, prefab, _nodeTransform, position);
            _nodes.Add(uinode);
            return uinode;
        }
    }
}
