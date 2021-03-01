using System;

namespace BattleSimulator.AI
{
    public class Wire
    {
        public Port from;
        public Port to;

        public Wire (Port from, Port to)
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
    }
}
