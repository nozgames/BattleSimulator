using UnityEngine;

namespace BattleSimulator.AI
{
    public abstract class Node
    {
        public Vector2 position { get; set; }

        public int lastExecutionId { get; set; }

        public abstract bool Execute(Context context);
    }
}
