
using System.Reflection;

namespace BattleSimulator.AI
{
    public enum NodePropertyType
    {
        Unknown,
        Float
    }

    public class NodeProperty
    {
        public string name { get; set; }
        public NodePropertyType type { get; set; }
        public PropertyInfo propertyInfo { get; set; }
    }
}
