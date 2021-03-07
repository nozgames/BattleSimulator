using System;
using System.Collections.Generic;

namespace BattleSimulator.AI
{
    public class PriorityInputPort : InputPort
    {
        public PriorityInputPort(Node node) : base(node)
        {
        }

        internal Priority Read(Context context, Func<List<Wire>, Priority> combiner = null)
        {
            if (wires.Count == 0)
                return Priority.none;

            Execute(context);

            if (null != combiner)
                return combiner(wires);

            if (wires.Count == 1)
                return wires[0].ReadPriority();

            // Default handling of multiple wires is to return the highest priority
            var result = Priority.none;
            foreach (var wire in wires)
            {
                var wirevalue = wire.ReadPriority();
                if (wirevalue > result)
                    result = wirevalue;
            }

            return result;
        }
    }
}
