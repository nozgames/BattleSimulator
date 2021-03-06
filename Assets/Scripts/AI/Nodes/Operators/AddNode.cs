using System.Collections.Generic;

namespace BattleSimulator.AI
{
    /// <summary>
    /// Add all input values 
    /// </summary>
    [Node(flags = NodeFlags.Compact)]
    public class AddNode : Node
    {
        [Port(flags = PortFlags.AllowMultipleWires)]
        public FloatInputPort inputs { get; private set; }
        public FloatOutputPort output { get; private set; }

        public AddNode()
        {
            inputs = new FloatInputPort(this);
            output = new FloatOutputPort(this);
        }

        private float Combiner (List<Wire> wires)
        {
            var result = 0.0f;
            foreach(var wire in wires)
                result += wire.ReadFloat();

            return result;
        }

        public override bool Execute(Context context)
        {
            output.value = inputs.Read(context, Combiner);
            return true;
        }
    }
}
