using System;

namespace BattleSimulator.AI
{
    public abstract class ActionNode : Node
    {
        public FloatPort priorityPort { get; private set; }

        public ActionNode()
        {
            priorityPort = new FloatPort(this, PortFlow.Input);
        }
    }
}
