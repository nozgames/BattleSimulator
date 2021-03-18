namespace BattleSimulator.Abilities
{
    [AbilityComponentMenu("Effect/ApplyEffectToTarget")]
    class ApplyEffectToTarget : ApplyEffect
    {
        public override void ToClient(Unit unit)
        {
            foreach(var effect in _effects)
            {
                effect.ToClient(unit);
            }
        }
    }
}
