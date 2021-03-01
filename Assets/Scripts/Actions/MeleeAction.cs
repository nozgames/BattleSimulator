using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleSimulator
{
    // TODO: base on attack action for all the common things such as health, class, etc
    class MeleeAction : UnitAction
    {
        [SerializeField] private float _rangeMin = 0.0f;
        [SerializeField] private float _rangeMax = 1.0f;

        public override bool isInRange => Unit.DistanceInRange(target, _rangeMin, _rangeMax);

        public override void Perform(float deltaTime)
        {
            
        }
    }
}
