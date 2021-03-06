﻿namespace BattleSimulator.AI
{
    class IsEnemyNode : BooleanValueNode
    {
        protected override bool GetValue(Context context) =>
            context.target != null && context.target.team != context.unit.team;
    }
}
