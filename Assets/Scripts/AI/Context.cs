
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.AI
{
    public class Context
    {
        public List<Target> _unitStack = new List<Target>();

        // TODO: action stack (current action)

        // TODO: this should be the second on the stack
        public Target unit => _unitStack[Mathf.Max(_unitStack.Count-2,0)];

        public Target target => _unitStack[_unitStack.Count - 1];

        public Target[] units { get; private set; }

        public Context(Target unit, Target[] units)
        {
            _unitStack.Add(unit);
            this.units = units;
        }

        public void PushTarget (Target target)
        {
            _unitStack.Add(target);
        }

        public void PopTarget ()
        {
            _unitStack.RemoveAt(_unitStack.Count - 1);
        }
    }
}
