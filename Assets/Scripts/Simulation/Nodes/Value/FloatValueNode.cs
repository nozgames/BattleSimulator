namespace BattleSimulator.Simulation
{
    public abstract class FloatValueNode : ValueNode
    {
        public FloatOutputPort output { get; private set; }

        public FloatValueNode()
        {
            output = new FloatOutputPort(this);
        }

        public sealed override bool Execute(Context context)
        {
            output.value = GetValue(context);
            return true;
        }

        protected abstract float GetValue(Context context);
    }
}
