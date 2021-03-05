using System;
using UnityEngine;

using BattleSimulator.AI;

namespace BattleSimulator.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }

        [Header("Colors")]
        [SerializeField] private Color _floatPortColor = Color.white;
        [SerializeField] private Color _priorityPortColor = Color.white;
        [SerializeField] private Color _unitPortColor = Color.white;
        [SerializeField] private Color _booleanPortColor = Color.white;
        [SerializeField] private Color _actionPortCoor = Color.white;

        public static Color GetPortColor (Port port)
        {
            var type = port?.GetType();

            if (type == typeof(FloatPort))
                return instance._floatPortColor;

            if (type == typeof(BooleanPort))
                return instance._booleanPortColor;

            if (type == typeof(TargetPort))
                return instance._unitPortColor;

            if (type == typeof(PriorityPort))
                return instance._priorityPortColor;

            //if (type == typeof(ActionPort))
            //    return instance._actionPortColor;

            return Color.gray;
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
