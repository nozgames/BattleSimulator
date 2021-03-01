using UnityEngine;

namespace BattleSimulator
{
    class TargetTypeFilter : TargetFilter
    {
        [SerializeField] private TargetType _targetType = TargetType.Unit;

        protected override bool Filter(Unit unit, Target target) => target.type == _targetType;
    }
}
