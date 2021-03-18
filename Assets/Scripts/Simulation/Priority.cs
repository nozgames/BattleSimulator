using System;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    public struct Priority
    {
        public static readonly Priority none = new Priority { value = 0.0f, weight = 0.0f };
        public static readonly Priority one = new Priority { value = 1.0f, weight = 1.0f };

        private float _value;

        public float value {
            get => _value;
            set {
                _value = Mathf.Clamp(value, 0, 1);
            }
        }

        public float weightedValue => weight * _value;

        public float weight;

        public static bool operator >(Priority lhs, Priority rhs) => lhs.weightedValue > rhs.weightedValue;
        public static bool operator <(Priority lhs, Priority rhs) => lhs.weightedValue < rhs.weightedValue;
    }
}
