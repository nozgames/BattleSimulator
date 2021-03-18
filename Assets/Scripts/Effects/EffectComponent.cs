using UnityEngine;

namespace BattleSimulator.Effects
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class EffectComponentMenuAttribute : System.Attribute
    {
        public string path { get; private set; }

        public EffectComponentMenuAttribute(string path)
        {
            this.path = path;
        }
    }

    public struct Modifier
    {
        public float amount;
        public float multiplier;
    }

    public class EffectComponent : ScriptableObject
    {
        [HideInInspector] [SerializeField] private bool _active = true;

        public virtual void ToClient (Unit unit)
        {

        }

        public virtual Modifier GetDamageModifier ()
        {
            return new Modifier { amount = 0, multiplier = 1.0f };
        }
    }
}
