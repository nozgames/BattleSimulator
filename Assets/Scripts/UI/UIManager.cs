using System;
using UnityEngine;

using BattleSimulator.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace BattleSimulator.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }

        [Header("General")]
        [SerializeField] private RectTransform _graphs = null;
        [SerializeField] private GraphicRaycaster _raycaster = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject _graphPrefab = null;

        [Header("Colors")]
        [SerializeField] private Color _floatPortColor = Color.white;
        [SerializeField] private Color _priorityPortColor = Color.white;
        [SerializeField] private Color _unitPortColor = Color.white;
        [SerializeField] private Color _booleanPortColor = Color.white;
        [SerializeField] private Color _actionPortCoor = Color.white;

        public static Color GetPortColor (PortInfo portInfo)
        {
            var type = portInfo.type;

            if (type == typeof(FloatInputPort) || type == typeof(FloatOutputPort))
                return instance._floatPortColor;

            if (type == typeof(BooleanInputPort) || type == typeof(BooleanOutputPort))
                return instance._booleanPortColor;

            if (type == typeof(UnitInputPort) || type == typeof(UnitOutputPort))
                return instance._unitPortColor;

            if (type == typeof(PriorityInputPort) || type == typeof(PriorityOutputPort))
                return instance._priorityPortColor;

            //if (type == typeof(ActionPort))
            //    return instance._actionPortColor;

            return Color.gray;
        }

        public static UIGraph NewGraph () => 
            UIGraph.Create(new AI.Graph(), instance._graphPrefab, instance._graphs);

        public static UIGraph LoadGraph (string path)
        {
            var graph = new Graph();
            graph.Load(path);

            return UIGraph.Create(graph, instance._graphPrefab, instance._graphs);
        }

        private void Awake()
        {
            instance = this;
        }

        public static int RayCast (Vector2 position, List<RaycastResult> results)
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = position;

            results.Clear();
            instance._raycaster.Raycast(eventData, results);
            return results.Count;
        }
    }
}
