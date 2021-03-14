using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    public class BooleanInputPort : InputPort
    {
        public BooleanInputPort (Node node) : base(node) { }

        internal bool Read(Context context, Func<List<Wire>, bool> combiner = null)
        {
            if (wires.Count == 0)
                return false;

            Execute(context);

            if (null != combiner)
                return combiner(wires);

            if (wires.Count == 1)
                return wires[0].ReadBoolean();

            // Default handling of multiple wires is a logical AND
            foreach (var wire in wires)
                if (!wire.ReadBoolean())
                    return false;

            return true;
        }
    }
}
