using UnityEngine;

namespace BattleSimulator.Effects
{
    [EffectComponentMenu("Damage/Damage")]
    class Damage : EffectComponent
    {
        [Header("General")]

        [Tooltip("Amount of damage to apply")]
        [SerializeField] private float _amount = 1.0f;

        [Tooltip("Number of times to apply the damage to the target")]
        [SerializeField] private int _count = 1;

        [Tooltip("Amount of time between damage ticks for damage over time")]
        [SerializeField] private float _interval = 0.0f;

        [Header("Radial")]
        [Tooltip("Damage multiplier when distance to center of radius is at its lowest value")]
        [SerializeField] private float _radialFalloffMin = 1.0f;

        [Tooltip("Damage multiplier when distance to center of radius is at its highest value")]
        [SerializeField] private float _radialFalloffMax = 1.0f;

        [Space]
        [Tooltip("Tags to identify damage type for resistances, etc")]
        [SerializeField] private Tag[] _tags;
    }
}
