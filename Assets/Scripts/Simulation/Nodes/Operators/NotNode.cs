using System.Collections.Generic;

namespace BattleSimulator.Simulation
{
    [Node(flags = NodeFlags.Compact)]
    class NotNode : Node
    {
        [Port(flags = PortFlags.AllowMultipleWires)]
        public BooleanInputPort input { get; private set; }
        public BooleanOutputPort output { get; private set; }

        public NotNode()
        {
            input = new BooleanInputPort(this);
            output = new BooleanOutputPort(this);
        }

        private bool Combiner(List<Wire> wires)
        {
            foreach (var wire in wires)
                if (wire.ReadBoolean())
                    return false;

            return true;
        }

        public override bool Execute(Context context)
        {
            output.value = input.Read(context, Combiner);
            return true;
        }
    }
}
