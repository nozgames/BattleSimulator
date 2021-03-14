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

    public class EffectComponent : ScriptableObject
    {
        [HideInInspector] [SerializeField] private bool _active = true;
    }
}
