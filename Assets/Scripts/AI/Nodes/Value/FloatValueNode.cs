﻿namespace BattleSimulator.AI
{
    public abstract class FloatValueNode : ValueNode
    {
        public FloatPort output { get; private set; }

        public FloatValueNode()
        {
            output = new FloatPort(this, PortFlow.Output);
        }

        public sealed override void Execute(Context context) => output.Write(GetValue(context));

        protected abstract float GetValue(Context context);
    }
}