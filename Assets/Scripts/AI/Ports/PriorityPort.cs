namespace BattleSimulator.AI
{
    public class PriorityPort : Port
    {
        public Priority value { get; private set; }

        public PriorityPort(Node node, PortFlow flow) : base (node, flow)
        {            
        }

        public void Write(float value) => this.value = new Priority { value = value, weight = 1.0f };

        public void Write(Priority value) => this.value = value;

        protected override float ReadFloat() => value.value;

        protected override Priority ReadPriority() => value;

    }
}
