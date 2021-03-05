namespace BattleSimulator.AI
{
    /// <summary>
    /// Add all input values 
    /// </summary>
    [Node(flags = NodeFlags.Compressed)]
    public class AddNode : Node
    {
        [Port(flags = PortFlags.AllowMultipleWires)]
        public FloatPort inputs { get; private set; }
        public FloatPort output { get; private set; }

        public AddNode()
        {
            inputs = new FloatPort(this, PortFlow.Input);
            output = new FloatPort(this, PortFlow.Output);
        }

        public override bool Execute(Context context)
        {
            var value = 0.0f;
            foreach (var wire in inputs._wires)
                value += wire.ReadFloat(context);

            output.Write(value);

            return true;
        }
    }
}
