
namespace BattleSimulator.Simulation
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
            var executionId = context.executionId;
            foreach (var wire in wires)
            {
                if (wire.from.node.lastExecutionId == executionId)
                    continue;

                wire.from.node.lastExecutionId = executionId;
                wire.from.node.Execute(context);
            }
        }
    }
}
