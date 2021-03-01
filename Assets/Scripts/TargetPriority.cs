using UnityEngine;

namespace BattleSimulator
{
    abstract class TargetPriority : MonoBehaviour
    {
        [SerializeField] private float _weight = 1.0f;

        public float weight => _weight;

        public abstract float GetPriority(Unit unit, Target target);
    }
}
