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

        private void Awake()
        {
            instance = this;
        }
    }
}
