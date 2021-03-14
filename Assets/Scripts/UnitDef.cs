using System;
using System.Linq;
using UnityEngine;

using BattleSimulator.Abilities;

namespace BattleSimulator
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "BattleSimulator/Unit")]
    public class UnitDef : ScriptableObjectWithGuid
    {
        [SerializeField] private Ability[] _abilities = null;

        public Ability[] abilities => _abilities;

        public Ability GetAbility(Guid id) => _abilities.FirstOrDefault(a => a.guid == id);
    }
}
