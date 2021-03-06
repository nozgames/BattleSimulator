using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.AI
{
    [Flags]
    public enum NodeFlags
    {
        None = 0,
        Compact = 1,
        Hidden = 2
    }

    public class NodeAttribute : Attribute
    {
        public NodeFlags flags { get; set; }
        public string name { get; set; }
    }

    public class NodeInfo
    {
        private static Dictionary<Type,NodeInfo> _cache = new Dictionary<Type,NodeInfo>();

        public Type nodeType { get; private set; }

        public PortInfo[] ports { get; private set; }

        public string name { get; private set; }

        public NodeFlags flags { get; private set; }

        public static NodeInfo Create(Type type)
        {
            if (!typeof(Node).IsAssignableFrom(type))
                throw new ArgumentException("type must derived from Node");

            if (_cache.TryGetValue(type, out var nodeInfo))
                return nodeInfo;

            var node = (Node)Activator.CreateInstance(type);
            if (null == node)
                throw new ArgumentException($"failed to create node of type '{type.Name}'");

            return Create(node);
        }

        public static NodeInfo Create (Node node)
        {
            var type = node.GetType();
            if (_cache.TryGetValue(type, out var nodeInfo))
                return nodeInfo;

            // Create a new node info
            nodeInfo = new NodeInfo();
            nodeInfo.nodeType = type;
            nodeInfo.name = type.Name;            
            _cache[type] = nodeInfo;

            var ports = new List<PortInfo>();
            for(; type != typeof(Node); type = type.BaseType)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach(var property in properties)
                {
                    if (!typeof(Port).IsAssignableFrom(property.PropertyType))
                        continue;

                    ports.Add(PortInfo.Create(node, property));                    
                }
            }

            nodeInfo.ports = ports.ToArray();

            var attr = nodeInfo.nodeType.GetCustomAttribute<NodeAttribute>(true);
            if (null != attr)
            {
                nodeInfo.flags = attr.flags;
                nodeInfo.name = !string.IsNullOrEmpty(attr.name) ? attr.name : nodeInfo.name;
            }

            if (nodeInfo.name.EndsWith("Node", StringComparison.OrdinalIgnoreCase))
                nodeInfo.name = nodeInfo.name.Substring(0, nodeInfo.name.Length - 4);

            return nodeInfo;
        }
    }
}
