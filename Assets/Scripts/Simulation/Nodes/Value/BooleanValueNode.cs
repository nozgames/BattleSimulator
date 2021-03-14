namespace BattleSimulator.Simulation
{
    [Node(flags = NodeFlags.Compact)]
    public abstract class BooleanValueNode : ValueNode
    {
        public BooleanOutputPort output { get; private set; }

        public BooleanValueNode()
        {
            output = new BooleanOutputPort(this);
        }

        public sealed override bool Execute(Context context)
        {
            output.value = GetValue(context);
            return true;
        }

        protected abstract bool GetValue(Context context);
    }
}
