using System;
using System.Reflection;

namespace BattleSimulator.Simulation
{
    public enum NodePropertyType
    {
        Unknown,
        Float,
        String,
        Guid
    }

    [Flags]
    public enum NodePropertyFlags
    {
        None = 0,
        Hidden = 1
    }

    public class NodeProperty
    {
        public string name { get; private set; }
        public NodePropertyType type { get; private set; }
        public PropertyInfo propertyInfo { get; private set; }
        public NodePropertyFlags flags { get; private set; }

        public bool isHidden => (flags & NodePropertyFlags.Hidden) == NodePropertyFlags.Hidden;

        /// <summary>
        /// Create a NodeProperty from the given PropertyInfo
        /// </summary>
        /// <param name="propertyInfo">Source to create the NodeProperty with</param>
        /// <returns>New NodeProperty or null if the property could not be created.</returns>
        public static NodeProperty FromPropertyInfo(PropertyInfo propertyInfo)
        {
            var nodePropertyInfo = propertyInfo.GetCustomAttribute<NodePropertyInfo>();
            if (null == nodePropertyInfo)
                return null;

            NodePropertyType nodePropertyType;
            if (propertyInfo.PropertyType == typeof(float))
                nodePropertyType = NodePropertyType.Float;
            else if (propertyInfo.PropertyType == typeof(string))
                nodePropertyType = NodePropertyType.String;
            else if (propertyInfo.PropertyType == typeof(Guid))
                nodePropertyType = NodePropertyType.Guid;
            else
                return null;

            return new NodeProperty {
                name = propertyInfo.Name,
                propertyInfo = propertyInfo,
                type = nodePropertyType,
                flags = nodePropertyInfo.Flags
            };
        }
    }
}
