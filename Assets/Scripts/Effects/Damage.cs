using UnityEngine;

namespace BattleSimulator.Effects
{
    [EffectComponentMenu("Damage/Damage")]
    class Damage : EffectComponent
    {
        [Header("General")]

        [Tooltip("Amount of damage to apply")]
        [SerializeField] private float _amount = 1.0f;

        [Tooltip("Number of times to apply the damage to the target")]
        [SerializeField] private int _count = 1;

        [Tooltip("Amount of time between damage ticks for damage over time")]
        [SerializeField] private float _interval = 0.0f;

        [Header("Radial")]
        [Tooltip("Damage multiplier when distance to center of radius is at its lowest value")]
        [SerializeField] private float _radialFalloffMin = 1.0f;

        [Tooltip("Damage multiplier when distance to center of radius is at its highest value")]
        [SerializeField] private float _radialFalloffMax = 1.0f;

        [Space]
        [Tooltip("Tags to identify damage type for resistances, etc")]
        [SerializeField] private Tag[] _tags;

        // TODO: effect runs on a unit with client side events.  
        // TODO: a wait that contains a presentation function will add an event into the
        //       runtime effect that will fire back to the presentation layer to do something. 
        //       each wait in the effect gets its own runtime event associated with it that the 
        //       server will fire.
#if false
        public void ToServer (AI.Unit unit)
        {
            // TODO: we need to handle damage modifier effects, so this really should call some sort
            //       of calculate damage method.  
            unit.health -= _amount;

            // TODO: handle death here.
        }
#endif

        public override void ToClient(Unit unit)
        {
            base.ToClient(unit);

            unit.Damage(_amount);
        }
    }
}
