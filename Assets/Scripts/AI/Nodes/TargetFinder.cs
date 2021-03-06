namespace BattleSimulator.AI
{
    public class TargetFinder : Node
    {
        public PriorityInputPort priorityPort { get; private set; }
        public BooleanInputPort filterPort { get; private set; }

        [Port(name = "priority")]
        public PriorityOutputPort targetPriorityPort { get; private set; }
        public UnitOutputPort targetPort { get; private set; }

        public TargetFinder()
        {
            priorityPort = new PriorityInputPort(this);
            filterPort = new BooleanInputPort(this);
            targetPort = new UnitOutputPort(this);
            targetPriorityPort = new PriorityOutputPort(this);
        }

        public override bool Execute(Context context)
        {
            var bestPriority = Priority.none;
            var bestTarget = (Target)null;

            foreach(var target in context.units)
            {
                context.PushTarget(target);

                if(!filterPort.Read(context))
                {
                    context.PopTarget();
                    continue;
                }

                var p = priorityPort.Read(context);
                if(p.value * p.weight > bestPriority.value * bestPriority.weight)
                {
                    bestPriority = p;
                    bestTarget = target;
                }
                
                context.PopTarget();
            }

            targetPriorityPort.value = bestPriority;
            targetPort.value = bestTarget;

            return true;
        }
    }
}

