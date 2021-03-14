using UnityEngine;

namespace BattleSimulator.Abilities
{
    class Range : AbilityComponent
    {
        [Tooltip("Minimum range required to use this ability")]
        [SerializeField] private float _min = 0.0f;

        [Tooltip("Maximum range required to use this ability")]
        [SerializeField] private float _max = 1.0f;

        public virtual bool Filter(Unit unit, Unit target)
        {
            if (target == null)
                return false;

            var distanceSqr = unit.DistanceToSqr(target);
            return distanceSqr >= _min * _min && distanceSqr <= _max * _max;
        }
    }
}
