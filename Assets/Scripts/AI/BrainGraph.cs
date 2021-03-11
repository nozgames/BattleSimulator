using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSimulator.AI
{
    /// <summary>
    /// Graph that chooses an ability for a unit based on priority
    /// </summary>
    public class BrainGraph : Graph
    {
        private ActionNode[] _actions;

        public override void Compile()
        {
            _actions = nodes.OfType<ActionNode>().ToArray();            
        }

        public AI.Target Execute(Context context)
        {
            if(_actions == null)
                Compile();

            foreach (var node in nodes)
                node.lastExecutionId = -1;

            foreach (var action in _actions)
            {
                action.Execute(context);
            }

            var bestPriority = Priority.none;
            var bestAction = (ActionNode)null;
            foreach (var action in _actions)
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

            if (bestAction is ActionNodeWithTarget actionWithTarget)
                return actionWithTarget.target;

            return null;
        }
    }
}
