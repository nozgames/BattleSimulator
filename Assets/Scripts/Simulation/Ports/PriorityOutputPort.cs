namespace BattleSimulator.Simulation
{
    public class PriorityOutputPort : OutputPort
    {
        public Priority value { get; set; }

        public PriorityOutputPort(Node node) : base(node) { }

        internal override float ReadFloat() => value.weightedValue;
        internal override bool ReadBoolean() => ReadFloat() != 0.0f;
        internal override Priority ReadPriority() => value;
    }
}

