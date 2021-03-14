
namespace BattleSimulator.Simulation
{
    public abstract class OutputPort : Port
    {
        public OutputPort(Node node) : base(node, PortFlow.Output) { }

        internal virtual float ReadFloat() => 0.0f;
        internal virtual bool ReadBoolean() => false;
        internal virtual Target ReadUnit () => null;
        internal virtual Priority ReadPriority() => Priority.none;
    }
}
