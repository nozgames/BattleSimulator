using System;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    [Node(prefab = "ConstFloatNode")]
    class ConstFloatNode : FloatValueNode
    {
        [SerializeField]
        public float value { get; set; }

        protected override float GetValue(Context context) => value;
    }
}
