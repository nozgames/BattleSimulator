using System.Collections.Generic;

namespace BattleSimulator.AI
{
    [Node(flags = NodeFlags.Compact)]
    class OrNode : Node
    {
        public BooleanInputPort input { get; private set; }
        public BooleanOutputPort output { get; private set; }

        public OrNode()
        {
            input = new BooleanInputPort(this);
            output = new BooleanOutputPort(this);
        }

        private bool Combiner (List<Wire> wires)
        {
            foreach (var wire in wires)
                if (wire.ReadBoolean())
                    return true;

            return false;
        }

        public override bool Execute(Context context)
        {
            output.value = input.Read(context, Combiner);
            return true;
        }
    }
}
