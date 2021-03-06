using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.AI
{
    public class BooleanInputPort : InputPort
    {
        public BooleanInputPort (Node node) : base(node) { }

        internal bool Read(Context context, Func<List<Wire>, bool> combiner = null)
        {
            if (_wires.Count == 0)
                return false;

            Execute(context);

            if (null != combiner)
                return combiner(_wires);

            if (_wires.Count == 1)
                return _wires[0].ReadBoolean();

            // Default handling of multiple wires is a logical AND
            foreach (var wire in _wires)
                if (!wire.ReadBoolean())
                    return false;

            return true;
        }
    }
}
