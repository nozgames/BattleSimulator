using System;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator
{
    class RetreatAction : UnitAction
    {
        [SerializeField] private HealthPriority _healthPriority;
        [SerializeField] private ClosestEnemyPriority _closestEnemyPriority;

        [Header("General")]
        [SerializeField] private float _totalPriority = 1.0f;

        public override void Perform(float deltaTime)
        {
            
        }
    }
}
