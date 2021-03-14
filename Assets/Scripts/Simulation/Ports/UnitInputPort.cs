using System;
using System.Collections.Generic;

namespace BattleSimulator.Simulation
{
    public class UnitInputPort : InputPort
    {
        public UnitInputPort(Node node) : base(node)
        {
        }

        internal Target Read(Context context, Func<List<Wire>, Target> combiner = null)
        {
            if (wires.Count == 0)
                return null;

            Execute(context);

            if (null != combiner)
                return combiner(wires);

            // Default handling of multiple targets is to choose the first one
            // TODO: could do closest or something
            return wires[0].ReadUnit();
        }
    }
}
