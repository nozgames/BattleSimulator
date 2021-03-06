using System;

namespace BattleSimulator.AI
{
    //[Node(flags = NodeFlags.Hidden)]
    public class ActionNode : Node
    {
        // TODO: just give action name rather than having it be the real action, do that elsewhere.

        public PriorityInputPort priorityPort { get; private set; }

        public string name { get; private set; }
        
        /// <summary>
        /// Total cooldown
        /// </summary>
        public float cooldown { get; set; }

        /// <summary>
        /// Total cooldown remaining (zero means not in cooldown)
        /// </summary>
        public float cooldownRemaining { get; set; }

        public Priority priority { get; protected set; }

        /// <summary>
        /// True if the action can be performed
        /// </summary>
        public bool canPerform => cooldownRemaining <= 0.0f;

        public ActionNode()
        {
            priorityPort = new PriorityInputPort (this);
        }

        public override bool Execute(Context context)
        {
            // If the action cannot be performed then give it no priority
            if(!canPerform)
            {
                priority = Priority.none;
                return false;
            }

            priority = priorityPort.Read(context);

            return true;
        }

        public void Perform() { }
    }
}
