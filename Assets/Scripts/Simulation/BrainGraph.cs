using System.IO;
using System.Linq;

namespace BattleSimulator.Simulation
{
    /// <summary>
    /// Graph that chooses an ability for a unit based on priority
    /// </summary>
    public class BrainGraph : Graph
    {
        private AbilityNode[] _abilities;

        private BrainGraph ()
        {
        }

        public BrainGraph(System.Guid unitDef)
        {
            this.unitDef = unitDef;
        }

        public override void Compile()
        {
            _abilities = nodes.OfType<AbilityNode>().ToArray();            
        }

        public (System.Guid,Target) Execute(Context context)
        {
            if(_abilities == null)
                Compile();

            foreach (var node in nodes)
                node.lastExecutionId = -1;

            foreach (var action in _abilities)
            {
                action.Execute(context);
            }

            var bestPriority = Priority.none;
            var bestAction = (AbilityNode)null;
            foreach (var action in _abilities)
            {
                action.Execute(context);

                if (action.priority > bestPriority)
                {
                    bestAction = action;
                    bestPriority = action.priority;
                }
            }

            if (bestAction != null)
            {
                // TODO: perform the action somehow..  Probably attach some data to the action that 
                //       the caller can use to determine what action to perform.  Also need to trigger the cooldown as well.

                //bestAction.Perform();
            }

            if (bestAction is AbilityNodeWithTarget actionWithTarget)
                return (bestAction.guid, actionWithTarget.target);

            return (System.Guid.Empty,null);
        }

        public static Graph Load(string path)
        {
            var graph = new BrainGraph();
            using (var file = File.OpenRead(path))
            using (var reader = new BinaryReader(file))
                graph.Load(reader);

            return graph;
        }
    }
}
