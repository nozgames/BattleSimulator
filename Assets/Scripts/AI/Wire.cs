using System;

namespace BattleSimulator.AI
{
    public class Wire
    {
        public OutputPort from;
        public InputPort to;

        public Wire (OutputPort from, InputPort to)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            if (to == null)
                throw new ArgumentNullException("to");

            if (from.flow != PortFlow.Output)
                throw new InvalidOperationException("wire.from port must be an Output port");

            if (to.flow != PortFlow.Input)
                throw new InvalidOperationException("wire.to port cannot be an Input port");

            this.from = from;
            this.to = to;
        }

        public float ReadFloat() => from.ReadFloat();

        public bool ReadBoolean () => from.ReadBoolean();

        public Priority ReadPriority() => from.ReadPriority();

        public Target ReadUnit() => from.ReadUnit();
    }
}
