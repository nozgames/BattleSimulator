using System;
using System.Reflection;

namespace BattleSimulator.AI
{
    [Flags]
    public enum PortFlags
    {
        None = 0,
        AllowMultipleWires = 1
    }

    public class PortAttribute : Attribute
    {
        public PortFlags flags { get; set; }
        public string name { get; set; }
    }

    public class PortInfo
    {
        public PortFlow flow;
        public Type type;
        public PortFlags flags;
        public string name;
        public NodeInfo nodeInfo;
        public PropertyInfo property;

        public static PortInfo Create (Node node, PropertyInfo property) 
        {
            var port = (Port)property.GetValue(node);
            if (null == port)
                throw new NullReferenceException($"port {property.Name} is null");

            var portInfo = new PortInfo();
            portInfo.type = port.GetType();
            portInfo.flow = port.flow;
            portInfo.name = property.Name;
            portInfo.flags = PortFlags.None;
            portInfo.nodeInfo = NodeInfo.Create(node);
            portInfo.property = property;

            var attr = property.GetCustomAttribute<PortAttribute>();
            if(attr != null)
            {
                portInfo.flags = attr.flags;
                portInfo.name = !string.IsNullOrEmpty(attr.name) ? attr.name : portInfo.name;
            }

            return portInfo;
        }
    }
}
