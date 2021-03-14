using System.Collections.Generic;

namespace BattleSimulator.Simulation
{
    /// <summary>
    /// Add all input values 
    /// </summary>
    [Node(flags = NodeFlags.Compact)]
    public class OneMinus : Node
    {
        public FloatInputPort input { get; private set; }
        public FloatOutputPort output { get; private set; }

        public OneMinus()
        {
            input = new FloatInputPort(this);
            output = new FloatOutputPort(this);
        }

        public override bool Execute(Context context)
        {
            output.value = 1.0f - input.Read(context);
            return true;
        }
    }
}
