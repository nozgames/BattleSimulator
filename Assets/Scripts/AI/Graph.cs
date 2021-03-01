namespace BattleSimulator.AI
{
    public class Graph
    {
        private Node[] _nodes;

        // TODO: This could be build from the node list afer loading a graph
        private ActionNode[] _actions; 

        // TODO: what parameters
        public void Execute (Context context)
        {
            foreach(var action in _actions)
            {
                action.Execute(context);
            }
        }

        /* TODO
        public void Compile()
        {
            // TODO: we could compile the graph such that we build an array of the nodes that need to be evaluated in a particular order
        }
        */
    }
}
