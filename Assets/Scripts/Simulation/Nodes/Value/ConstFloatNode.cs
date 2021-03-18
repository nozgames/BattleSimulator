using System;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    [Node(prefab = "ConstFloatNode")]
    class ConstFloatNode : FloatValueNode
    {
        [NodePropertyInfo]
        public float value { get; set; }

        protected override float GetValue(Context context) => value;
    }
}
