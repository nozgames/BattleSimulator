using System;

namespace BattleSimulator.AI
{
    public abstract class ActionNodeWithTarget : ActionNode
    {
        public TargetPort targetPort { get; private set; }

        public ActionNodeWithTarget ()
        {
            targetPort = new TargetPort(this, PortFlow.Input);
        }
    }
}
