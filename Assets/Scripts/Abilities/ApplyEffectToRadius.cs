using UnityEngine;

namespace BattleSimulator.Abilities
{
    [AbilityComponentMenu("Effect/ApplyEffectToRadius")]
    class ApplyEffectToRadius : ApplyEffect
    {
        [Header("Shape", order = 0)]
        [Tooltip("Radius to apply the effect in")]
        [SerializeField] private float _radius = 1.0f;

        [Tooltip("Arc to apply the effect in (360 for ensure radius")]
        [SerializeField] private float _arc = 360.0f;

        [Header("Filters", order = 1)]
        [Tooltip("True if the unity sehould be inclued")]
        [SerializeField] private bool _self = false;

        [Tooltip("True if the current target should be included")]
        [SerializeField] private bool _target = false;

        [Tooltip("True if enemy targets should be included")]
        [SerializeField] private bool _enemy = false;

        [Tooltip("True if friendly targets should be included")]
        [SerializeField] private bool _friendly = false;
    }
}
