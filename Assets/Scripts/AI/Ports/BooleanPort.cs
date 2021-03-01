namespace BattleSimulator.AI
{
    public class BooleanPort : Port
    {
        public bool value { get; set; }

        public BooleanPort (Node node, PortFlow flow, bool value = false) : base (node, flow)
        {
            this.value = value;
        }
    }
}
