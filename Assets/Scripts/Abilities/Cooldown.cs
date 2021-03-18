using UnityEngine;

namespace BattleSimulator.Abilities
{
    public class Cooldown : AbilityComponent
    {
        [SerializeField] private float _duration = 1.0f;
    }
}
