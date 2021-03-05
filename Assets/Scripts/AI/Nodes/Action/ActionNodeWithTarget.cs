using System;

namespace BattleSimulator.AI
{
    public class ActionNodeWithTarget : ActionNode
    {
        public TargetPort targetPort { get; private set; }

        public ActionNodeWithTarget (string name) : base(name)
        {
            targetPort = new TargetPort(this, PortFlow.Input);
        }

        public override bool Execute(Context context)
        {
            // Update the priority and check cooldowns, etc.
            if (!base.Execute(context))
                return false;

            // Force the target port to update its value
            targetPort.ReadTarget(context);

            // If no target was given then fail.
            if(targetPort.value == null)
            {
                priorityPort.Write(Priority.none);
                return false;
            }

            return true;
        }
    }
}
