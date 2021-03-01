using UnityEngine;

namespace BattleSimulator
{
    public enum TargetType
    {
        Unit,
        Projectile
    }

    public abstract class Target : MonoBehaviour
    {
        public TargetType type => TargetType.Unit;

        public float DistanceTo(Target target) => (target.transform.position - transform.position).magnitude;
        public float DistanceTo(Vector3 position) => (transform.position - position).magnitude;

        public float DistanceToSqr(Target target) => (target.transform.position - transform.position).sqrMagnitude;

        /// <summary>
        /// Returns true if the distance to the given target is within the given range
        /// </summary>
        /// <param name="target">Target to compare distance to</param>
        /// <param name="min">Minimum distance</param>
        /// <param name="max">Maximum distance</param>
        /// <returns>True if the given target is within the given distance range.</returns>
        public bool DistanceInRange (Target target, float min, float max)
        {
            if (target == null)
                return false;

            var distSqr = DistanceToSqr(target);
            return (distSqr >= min * min && distSqr <= max * max);
        }
    }
}
