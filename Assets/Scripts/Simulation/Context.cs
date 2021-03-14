
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace BattleSimulator.Simulation
{
    public class Context
    {
        // TODO: list of abilities for the unit
        

        public List<int> _unitStack = new List<int>();

        // TODO: action stack (current action)

        // TODO: this should be the second on the stack
        public Target unit => units[unitIndex];

        public int unitIndex => _unitStack[Mathf.Max(_unitStack.Count - 2, 0)];

        public int targetIndex => _unitStack[_unitStack.Count - 1];

        public Target target => units[targetIndex];

        public int executionId => (unitIndex << 16) + targetIndex;

        public Target[] units { get; private set; }

        public Context(int unit, Target[] units)
        {
            _unitStack.Add(unit);
            this.units = units;
        }

        public void PushUnit (int unit)
        {
            if (unit < 0 || unit >= units.Length)
                return;

            _unitStack.Add(unit);
        }

        public void PopTarget ()
        {
            _unitStack.RemoveAt(_unitStack.Count - 1);
        }
    }
}
