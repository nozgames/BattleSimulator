using UnityEngine;

namespace BattleSimulator.Simulation
{
    public struct Stat
    {
        public float unmodified;
        public float modified;
    }

    public class Unit
    {
        public Vector3 position;
        public float rotation;
        public float health;
        public float maxHealth;
        public int team;

        public Stat attack;
        public Stat defense;

        // TODO: link back to the client side unit, identifier?

        // TODO: list of effects
        // TODO: list of abilities

        // TODO: graph?
    }
}
