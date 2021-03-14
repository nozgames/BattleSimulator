using UnityEngine;

namespace BattleSimulator.Abilities
{
    [AbilityComponentMenu("Wait/WaitForAnimationEvent")]
    class WaitForAnimationEvent : WaitComponent
    {
        [SerializeField] private Tag _event = null;
    }
}
