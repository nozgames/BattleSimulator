using UnityEngine;

namespace BattleSimulator.Abilities
{
    class Range : AbilityComponent
    {
        [Tooltip("Minimum range required to use this ability")]
        [SerializeField] private float _min = 0.0f;

        [Tooltip("Maximum range required to use this ability")]
        [SerializeField] private float _max = 1.0f;

        public override bool CanPerform(Unit unit, Unit target)
        {
            if (target == null)
                return false;
            
            var distance = Mathf.Max(0,unit.DistanceTo(target) - unit.size - target.size);
            return distance >= _min && distance <= _max;
        }
    }
}
