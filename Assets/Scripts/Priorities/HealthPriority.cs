using UnityEngine;
using Unity.Mathematics;

namespace BattleSimulator
{
    public class HealthPriority : UnitActionPriority
    {
        [SerializeField] private float _healthMin = 0.0f;
        [SerializeField] private float _healthMax = 1.0f;

        public override float CalculatePriority(Unit unit) =>
            math.clamp(math.remap(_healthMin, _healthMax, 0, 1, unit.NormalizedHealth), 0, 1);
    }
}
