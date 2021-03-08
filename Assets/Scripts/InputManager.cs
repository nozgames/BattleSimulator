
using UnityEngine;

namespace BattleSimulator
{
    // TODO: maintain mouse coordinates for cursor

    /// <summary>
    /// Handles all input within the game
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {

        }
    }
}
