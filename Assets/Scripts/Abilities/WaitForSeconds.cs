using UnityEngine;

namespace BattleSimulator.Abilities
{
    [AbilityComponentMenu("Wait/WaitForSeconds")]
    class WaitForSeconds : WaitComponent
    {
        [SerializeField] private float _seconds = 1.0f;
    }
}
