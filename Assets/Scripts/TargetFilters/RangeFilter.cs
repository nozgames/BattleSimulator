

using UnityEngine;

namespace BattleSimulator
{
    class RangeFilter : TargetFilter
    {
        [SerializeField] private float _rangeMin = 0.0f;
        [SerializeField] private float _rangeMax = 1.0f;

        protected override bool Filter(Unit unit, Target target)
        {
            var dist = unit.DistanceToSqr(target);
            return dist >= _rangeMin && dist <= _rangeMax;
        }
    }
}
