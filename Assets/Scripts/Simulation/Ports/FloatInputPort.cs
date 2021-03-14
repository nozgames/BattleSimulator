using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    public class FloatInputPort : InputPort
    {
        public FloatInputPort(Node node) : base(node)
        {
        }

        internal float Read(Context context, Func<List<Wire>, float> combiner = null, float defaultValue = 0.0f)
        {
            if (wires.Count == 0)
                return defaultValue;

            Execute(context);

            if (null != combiner)
                return combiner(wires);

            if (wires.Count == 1)
                return wires[0].ReadFloat();

            // Default handling of multiple wires is to return the value with the greatest magnitude
            var value = 0.0f;
            var absvalue = 0.0f;
            foreach (var wire in wires)
            {
                var wirevalue = wire.ReadFloat();
                var wireabsvalue = Mathf.Abs(wirevalue);
                if (wireabsvalue > absvalue)
                {
                    absvalue = wireabsvalue;
                    value = wirevalue;
                }
            }

            return value;
        }
    }
}
