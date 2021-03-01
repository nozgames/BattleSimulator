
namespace BattleSimulator
{
    class EnemyTargetFilter : TargetFilter
    {
        protected override bool Filter(Unit unit, Target target) =>
            target.type == TargetType.Unit && unit.Team != ((Unit)target).Team;
    }
}
