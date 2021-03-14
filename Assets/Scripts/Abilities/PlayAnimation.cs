using UnityEngine;

namespace BattleSimulator.Abilities
{
    class PlayAnimation : AbilityComponent
    {
        [SerializeField] private string _name = null;

        public override void ToClient(Unit unit)
        {
            if(unit.GetComponent<Animator>() != null)
                unit.GetComponent<Animator>().Play(_name, 0, 0);
        }
    }
}
