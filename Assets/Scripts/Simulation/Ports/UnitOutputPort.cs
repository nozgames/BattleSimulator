namespace BattleSimulator.Simulation
{
    public class UnitOutputPort : OutputPort
    {
        public Target value { get; set; }

        public UnitOutputPort (Node node) : base(node) { }

        internal override Target ReadUnit() => value;
    }
}
