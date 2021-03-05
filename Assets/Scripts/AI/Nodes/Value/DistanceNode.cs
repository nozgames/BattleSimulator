namespace BattleSimulator.AI
{
    /// <summary>
    /// Provides the distance between the current unit and the current target
    /// </summary>
    public class DistanceNode : FloatValueNode
    {
        protected override float GetValue(Context context) => 
            (context.target.position - context.unit.position).magnitude;
    }
}
