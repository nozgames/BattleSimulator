using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BattleSimulator.UI;
using BattleSimulator.AI;

namespace BattleSimulator
{
    public class UIGraph : MonoBehaviour, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [Header("General")]
        [SerializeField] private RectTransform _zoomTransform = null;
        [SerializeField] private UIGrid _grid = null;
        [SerializeField] private RectTransform _nodeTransform = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject _nodePrefab = null;
        [SerializeField] private GameObject _compressedNodePrefab = null;

        private int _zoomLevel = 10;

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

        public void OnPointerDown(PointerEventData eventData)
        {
           
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        private Vector2 start;
        private Vector2 anchorStart;

        public void OnBeginDrag(PointerEventData eventData)
        {
            start = eventData.position;
            anchorStart = _zoomTransform.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _zoomTransform.anchoredPosition = anchorStart + (eventData.position - start);
                _grid.GridOffset = _zoomTransform.anchoredPosition;
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
