using UnityEngine;

namespace BattleSimulator.Abilities
{
    class GlobalCooldown : AbilityComponent 
    {
        [SerializeField] private float _duration = 0.25f;

        public override void Execute(Unit unit)
        {
            unit.globalCooldown = _duration;
        }

        // TODO: This should be ToServer
        public override void ToClient(Unit unit)
        {
            unit.globalCooldown = _duration;
            base.ToClient(unit);
        }
    }
}
