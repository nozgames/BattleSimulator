using UnityEngine;

namespace BattleSimulator.AI
{
    /// <summary>
    /// Convert a float to a priority
    /// </summary>
    public class FloatToPriority : Node
    {
        public FloatPort input { get; private set; }
        public FloatPort min { get; private set; }
        public FloatPort max { get; private set; }
        public FloatPort weight { get; private set; }
        public PriorityPort output { get; private set; }

        public FloatToPriority()
        {
            input = new FloatPort(this, PortFlow.Input);
            min = new FloatPort(this, PortFlow.Input);
            max = new FloatPort(this, PortFlow.Input);
            weight = new FloatPort(this, PortFlow.Input);
            output = new PriorityPort(this, PortFlow.Output);
        }

        public override bool Execute(Context context)
        {
            var value = this.input.ReadFloat(context);
            var min = this.min.ReadFloat(context);
            var max = this.max.ReadFloat(context);
            var weight = this.weight.ReadFloat(context);

            output.Write(new Priority {
                weight = weight,
                value = Mathf.Clamp((value - min) / (max - min), 0, 1)
            });

            return true;
        }
    }
}
