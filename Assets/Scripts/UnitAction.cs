using UnityEngine;

namespace BattleSimulator
{
    public abstract class UnitAction : MonoBehaviour
    {
        [SerializeField] private float _cooldown = 0.0f;

        private Unit _unit;

        private float _priority = 0.0f;

        private Target _target = null;

        public Unit Unit => _unit;

        private UnitActionPriority[] _priorities;

        private TargetPriority[] _targetPriorities;

        /// <summary>
        /// Amount of time remaining in the cooldown
        /// </summary>
        private float _cooldownRemaining = 0.0f;

        private float cooldownNormalized => _cooldown <= 0.0f ? 0.0f : _cooldownRemaining / _cooldown;

        public float priority => _priority;

        public Target target => _target;

        public virtual bool isInRange => true;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _priorities = GetComponents<UnitActionPriority>();
        }

        public void CalculatePriority()
        {
            // Zero priority if the action is on cooldown
            if(_cooldownRemaining > 0)
            {
                _priority = 0.0f;
                return;
            }

            //foreach (var priority in _priorities)
              //  priority.Update(_unit);
        }

        public void FindTarget ()
        {
#if false
            for reach target 
                for each target priority
                    if priority is zero skip target
                    keep running total of weight and priority
                priority = total priority / total weight
                if priority is best
                    set target as best target
#endif
        }

        public abstract void Perform(float deltaTime);
    }
}
