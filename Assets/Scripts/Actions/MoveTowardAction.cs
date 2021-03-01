using UnityEngine;

namespace BattleSimulator
{
    class MoveTowardAction : UnitAction
    {
        [SerializeField] private float _speed = 1.0f;

        public override void Perform(float deltaTime)
        {
            if (!Unit.HasTarget)
                return;

            Unit.transform.rotation = Quaternion.LookRotation(Unit.DirectionToTarget(), Vector3.up);
            Unit.transform.position = Unit.transform.forward * _speed * deltaTime;
        }
    }
}
