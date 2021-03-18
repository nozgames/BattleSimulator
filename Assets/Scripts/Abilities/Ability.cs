using System;
using System.Linq;
using UnityEngine;

namespace BattleSimulator.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "BattleSimulator/Ability")]
    public class Ability : ScriptableObjectWithGuid
    {
        [SerializeField] private string _displayName;
        [SerializeField] private AbilityComponent[] _components = null;

        public string displayName => _displayName;

        public AbilityComponent[] components => _components;

        /// <summary>
        /// Returns true if the ability has the ability component of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if the component of the given type exists</returns>
        public bool Has(Type type) => _components?.Any(c => c.GetType() == type) ?? false;

        public virtual void ToPresentation(Unit unit)
        {
            foreach (var component in _components)
                component.ToClient(unit);

            // TODO: converts the ability to a presentation object that the presentation layer can use.  this is typically
            //       ability components that play animations, fx, etc.
        }

        public virtual void ToSimulation()
        {
            // TODO: this method converts the ability definition to a simulated object
            // TODO: for each component we attempt to convert to the simulation.
            // TODO: for example apply effect to radius may make an ApplyEffect simulation command that uses a RadiusTargetFinder 
            // TODO: keep simulation and presentation separate so we could run the simulation as a function from command line
            // TODO: could compile the scripts and save out the simulation state and just run that without needing any of the unity assets.
        }

        public bool CanPerform (Unit unit, Unit target)
        {
            foreach (var component in _components)
                if (!component.CanPerform(unit, target))
                    return false;

            return true;
        }
    }
}
