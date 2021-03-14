using UnityEngine;

using BattleSimulator.Effects;

namespace BattleSimulator.Abilities
{
    [AbilityComponentUsage(allowMultiple = true)]
    abstract class ApplyEffect : AbilityComponent
    {
        [SerializeField] protected Effect[] _effects = null;
    }
}
