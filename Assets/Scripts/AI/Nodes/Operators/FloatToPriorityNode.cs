using UnityEngine;

namespace BattleSimulator.AI
{
    /// <summary>
    /// Convert a float to a priority
    /// </summary>
    public class FloatToPriority : Node
    {
        public FloatInputPort inputPort { get; private set; }
        public FloatInputPort minPort { get; private set; }
        public FloatInputPort maxPort { get; private set; }
        public FloatInputPort weightPort { get; private set; }
        public PriorityOutputPort outputPort { get; private set; }

        public FloatToPriority()
        {
            inputPort = new FloatInputPort(this);
            minPort = new FloatInputPort(this);
            maxPort = new FloatInputPort(this);
            weightPort = new FloatInputPort(this);
            outputPort = new PriorityOutputPort(this);
        }

        public override bool Execute(Context context)
        {
            var value = inputPort.Read(context);
            var min = minPort.Read(context);
            var max = maxPort.Read(context);
            var weight = weightPort.Read(context, null, 1.0f);

            outputPort.value = new Priority {
                weight = weight,
                value = Mathf.Clamp((value - min) / (max - min), 0, 1)
            };

            return true;
        }
    }
}
