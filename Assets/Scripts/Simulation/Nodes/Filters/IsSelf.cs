namespace BattleSimulator.Simulation
{
    class IsSelf : BooleanValueNode
    {
        protected override bool GetValue(Context context) =>
            context.target == context.unit;
    }
}
