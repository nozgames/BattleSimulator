namespace BattleSimulator.Simulation
{
    public class HealthPercentageNode : FloatValueNode
    {
        protected override float GetValue(Context context) =>
            context.unit.health / context.unit.maxHealth;
    }
}
