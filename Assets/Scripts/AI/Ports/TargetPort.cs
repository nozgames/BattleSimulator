namespace BattleSimulator.AI
{
    public class TargetPort : Port
    {
        public Target value { get; set; }

        public TargetPort(Node node, PortFlow flow, Target value = null) : base (node, flow)
        {
            this.value = value;
        }
    }
}
