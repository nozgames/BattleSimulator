
namespace BattleSimulator.AI
{
    public abstract class InputPort : Port
    {
        public InputPort(Node node) : base(node, PortFlow.Input)
        {
        }

        /// <summary>
        /// Execute all nodes connected to this port
        /// </summary>
        protected void Execute(Context context)
        {
            foreach (var wire in wires)
                wire.from.node.Execute(context);
        }
    }
}
