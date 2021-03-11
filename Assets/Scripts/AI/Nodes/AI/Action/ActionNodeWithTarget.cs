using System;

namespace BattleSimulator.AI
{
    //[Node(flags = NodeFlags.Hidden)]
    public class ActionNodeWithTarget : ActionNode
    {
        public Target target { get; private set; }

        public UnitInputPort targetPort { get; private set; }

        public ActionNodeWithTarget ()
        {
            targetPort = new UnitInputPort(this);
        }

        public override bool Execute(Context context)
        {
            // Update the priority and check cooldowns, etc.
            if (!base.Execute(context))
                return false;

            // Force the target port to update its value
            target = targetPort.Read(context);

            // If no target was given then fail.
            if(target == null)
            {
                priority = Priority.none;
                return false;
            }

            return true;
        }
    }
}
