using System;

namespace BattleSimulator.Simulation
{
    public class NodePropertyInfo : Attribute
    {
        public NodePropertyFlags Flags { get; set; } = NodePropertyFlags.None;
    }
}
