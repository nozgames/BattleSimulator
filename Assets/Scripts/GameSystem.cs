using UnityEngine;

namespace BattleSimulator
{
    public class GameSystem : MonoBehaviour
    {
        private GameSystem _instance = null;

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            // Update all units
            Unit.UpdateAll();
        }
    }
}
