using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator
{
    [CreateAssetMenu(fileName = "New UnitDef", menuName = "Battle Simulator/UnitDef")]
    public class UnitDef : ScriptableObject
    {
        [SerializeReference] [SerializeField] public List<UnitActionDef> _actions = new List<UnitActionDef>();
    }
}
