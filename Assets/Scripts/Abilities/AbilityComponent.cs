using UnityEngine;

namespace BattleSimulator.Abilities
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AbilityComponentMenuAttribute : System.Attribute
    {
        public string path { get; private set; }

        public AbilityComponentMenuAttribute(string path)
        {
            this.path = path;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AbilityComponentUsageAttribute : System.Attribute
    {
        public bool allowMultiple { get; set; } = false;
    }

    public class AbilityComponent : ScriptableObject
    {
        [HideInInspector] [SerializeField] private bool _active = true;

        public bool isActive => _active;


        public virtual bool Filter (Unit unit)
        {
            return true;
        }


        public virtual void Execute (Unit unit)
        {
        }

        public virtual void ToSimulation ()
        {
            // TODO: this method converts the ability definition to a simulated object
        }

        public virtual void ToClient (Unit unit)
        {
        }
    }
}
