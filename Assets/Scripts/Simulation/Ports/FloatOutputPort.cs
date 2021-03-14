namespace BattleSimulator.Simulation
{
    public class FloatOutputPort : OutputPort
    {
        public float value { get; set; }

        public FloatOutputPort(Node node) : base(node)
        {
        }

        internal override float ReadFloat() => value;
        internal override bool ReadBoolean() => value != 0.0f;
        internal override Priority ReadPriority() => new Priority { value = value, weight = 1.0f };
    }
}

