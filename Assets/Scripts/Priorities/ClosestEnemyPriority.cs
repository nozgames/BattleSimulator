using UnityEngine;

namespace BattleSimulator
{
    public class ClosestEnemyPriority : UnitActionPriority
    {
        [SerializeField] private float _distanceMin = 0.0f;
        [SerializeField] private float _distanceMax = 1.0f;

        public override float CalculatePriority(Unit unit)
        {
            return 0.0f;
        }
    }
}
