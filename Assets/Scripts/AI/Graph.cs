using System.Collections.Generic;

namespace BattleSimulator.AI
{
    public class Graph
    {
        private List<Node> _nodes = new List<Node>();

        // TODO: This could be build from the node list afer loading a graph
        private List<ActionNode> _actions = new List<ActionNode>(); 

        // TODO: what parameters
        public void Execute (Context context)
        {            
            foreach(var action in _actions)
            {
                action.Execute(context);
            }

            var bestPriority = Priority.none;
            var bestAction = (ActionNode)null;
            foreach(var action in _actions)
            {
                action.Execute(context);

                if(action.priority > bestPriority)
                {
                    bestAction = action;
                    bestPriority = action.priority;
                }
            }

            if(bestAction != null)
            {
                // TODO: perform the action somehow..  Probably attach some data to the action that 
                //       the caller can use to determine what action to perform.  Also need to trigger the cooldown as well.

                bestAction.Perform();
            }
        }

        public void AddNode (Node node)
        {
            _nodes.Add(node);

            if (node is ActionNode actionNode)
                _actions.Add(actionNode);
        }

        /* TODO
        public void Compile()
        {
            // TODO: we could compile the graph such that we build an array of the nodes that need to be evaluated in a particular order
        }
        */
    }
}
