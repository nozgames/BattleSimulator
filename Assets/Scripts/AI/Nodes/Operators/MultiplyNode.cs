using System.Collections.Generic;

namespace BattleSimulator.AI
{
    /// <summary>
    /// Multiply all input values 
    /// </summary>
    [Node(flags = NodeFlags.Compact)]
    public class MultiplyNode : Node
    {
        [Port(flags = PortFlags.AllowMultipleWires)]
        public FloatInputPort inputs { get; private set; }
        public FloatOutputPort output { get; private set; }

        public MultiplyNode()
        {
            inputs = new FloatInputPort(this);
            output = new FloatOutputPort(this);
        }

        private float Combiner(List<Wire> wires)
        {
            var result = wires[0].ReadFloat();
            for(int i=1; i<wires.Count; i++)
                result *= wires[i].ReadFloat();

            return result;
        }

        public override bool Execute(Context context)
        {
            output.value = inputs.Read(context, Combiner);
            return true;
        }
    }
}
