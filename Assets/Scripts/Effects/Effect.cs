using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "BattleSimulator/Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField] private List<EffectComponent> _components = new List<EffectComponent>();

        public List<EffectComponent> components => _components;

        /// <summary>
        /// Returns true if the effect has the effect component of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if the component of the given type exists</returns>
        public bool Has(Type type) => _components.Any(c => c.GetType() == type);
    }
}
