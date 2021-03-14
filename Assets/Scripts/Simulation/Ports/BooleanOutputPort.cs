namespace BattleSimulator.Simulation
{
    public class BooleanOutputPort : OutputPort
    {
        public bool value { get; set; }

        public BooleanOutputPort(Node node) : base(node)
        {
        }

        internal override float ReadFloat() => value ? 1.0f : 0.0f;
        internal override bool ReadBoolean() => value;
        internal override Priority ReadPriority() => new Priority { value = ReadFloat(), weight = 1.0f };
    }
}

