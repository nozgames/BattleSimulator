using UnityEngine;

namespace BattleSimulator
{
    abstract class TargetFilter : TargetPriority
    {
        public override float GetPriority(Unit unit, Target target) => Filter(unit, target) ? 1.0f : 0.0f;

        protected abstract bool Filter(Unit unit, Target target);
    }
}
