namespace BattleSimulator.Simulation
{
    public class HealthNode : FloatValueNode
    {
        protected override float GetValue (Context context) => context.unit.health;
    }
}
