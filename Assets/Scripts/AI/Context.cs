
using System.Collections.Generic;

namespace BattleSimulator.AI
{
    public class Context
    {
        // TODO: unit stack
        public Stack<Target> _unitStack;

        // TODO: action stack (current action)

        // TODO: this should be the second on the stack
        public Target unit => _unitStack.Peek();

        public Target target => _unitStack.Peek();

        public Context(Target unit)
        {
            _unitStack.Push(unit);
        }
    }
}
