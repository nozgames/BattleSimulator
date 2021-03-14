using System;
using System.Collections.Generic;
using System.IO;

using BattleSimulator.Extensions;

namespace BattleSimulator.Simulation
{
    public abstract class Graph
    {
        private const int FileVersion = 2;

        private List<Node> _nodes = new List<Node>();

        public List<Node> nodes => _nodes;

        public abstract void Compile();

        public Guid unitDef { get; protected set; }

        /// <summary>
        /// Add a node to the graph
        /// </summary>
        /// <param name="node">Node to add</param>
        public void AddNode(Node node)
        {
            if (_nodes.Contains(node))
                return;

            _nodes.Add(node);
        }

        /// <summary>
        /// Remove a node from the graph
        /// </summary>
        /// <param name="node">Node to remove</param>
        public void RemoveNode(Node node)
        {
            _nodes.Remove(node);
        }

        public void Save(string filename)
        {
            using (var file = File.Create(filename))
            using (var writer = new BinaryWriter(file))
                Save(writer);
        }

        private void Save(BinaryWriter writer)
        {
            writer.WriteFourCC('B', 'B', 'G', 'R');
            writer.Write(FileVersion);
            writer.Write(unitDef);

            // Write the counts
            writer.Write(_nodes.Count);

            foreach (var node in _nodes)
            {
                writer.Write(node.GetType().FullName);
                writer.Write(node.position.x);
                writer.Write(node.position.y);
            }

            for (int nodeIndex = 0; nodeIndex < _nodes.Count; nodeIndex++)
            {
                var node = _nodes[nodeIndex];
                var nodeInfo = NodeInfo.Create(node);

                foreach (var property in nodeInfo.properties)
                {
                    writer.Write(nodeIndex);
                    writer.Write((byte)property.type);
                    writer.Write(property.propertyInfo.Name);

                    var value = property.propertyInfo.GetValue(node);
                    switch (property.type)
                    {
                        case NodePropertyType.Float:
                            writer.Write((float)value);
                            break;

                        case NodePropertyType.String:
                            writer.Write(value != null);
                            if (value != null)
                                writer.Write((string)value);
                            break;

                        case NodePropertyType.Guid:
                            writer.Write((Guid)value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            writer.Write(-1);

            // Write all wires
            for (int nodeIndex = 0; nodeIndex < _nodes.Count; nodeIndex++)
            {
                var node = _nodes[nodeIndex];
                var nodeInfo = NodeInfo.Create(node);

                foreach (var portInfo in nodeInfo.ports)
                {
                    if (portInfo.flow == PortFlow.Input)
                        continue;

                    var port = (OutputPort)portInfo.propertyInfo.GetValue(node);
                    foreach (var wire in port.wires)
                    {
                        writer.Write(nodeIndex);
                        writer.Write(portInfo.propertyInfo.Name);

                        var toNodeInfo = NodeInfo.Create(wire.to.node);
                        var toPortInfo = toNodeInfo.GetPortInfo(wire.to);
                        writer.Write(_nodes.IndexOf(wire.to.node));
                        writer.Write(toPortInfo.propertyInfo.Name);
                    }
                }
            }

            writer.Write(-1);
        }

        protected void Load (BinaryReader reader)
        {
            if (!reader.ReadFourCC('B', 'B', 'G', 'R'))
                throw new InvalidDataException("not a graph");

            var version = reader.ReadInt32();
            if (version != FileVersion)
                throw new InvalidDataException("unsupported version");

            unitDef = reader.ReadGuid();

            var nodeCount = reader.ReadInt32();
            _nodes.Clear();
            _nodes.Capacity = nodeCount;
            for(int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeType = Type.GetType(reader.ReadString());
                var node = (Node)Activator.CreateInstance(nodeType);
                node.position = new UnityEngine.Vector2(reader.ReadSingle(), reader.ReadSingle());
                AddNode(node);
            }

            // Read all node properties
            while(true)
            {
                var nodeIndex = reader.ReadInt32();
                if (nodeIndex == -1)
                    break;

                var node = _nodes[nodeIndex];
                var nodeInfo = NodeInfo.Create(node);
                var propertyType = (NodePropertyType)reader.ReadByte();
                var nodeProperty = nodeInfo.GetProperty(reader.ReadString());

                switch (propertyType)
                {
                    case NodePropertyType.Float:
                        nodeProperty.propertyInfo.SetValue(node, reader.ReadSingle());
                        break;

                    case NodePropertyType.String:
                        nodeProperty.propertyInfo.SetValue(node, reader.ReadString());
                        break;

                    case NodePropertyType.Guid:
                        nodeProperty.propertyInfo.SetValue(node, reader.ReadGuid());
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            // Read all wires
            while(true)
            {
                var nodeIndex = reader.ReadInt32();
                if (nodeIndex == -1)
                    break;

                var fromNode = _nodes[nodeIndex];
                var fromNodeInfo = NodeInfo.Create(fromNode);
                var fromPortInfo = fromNodeInfo.GetPortInfo(reader.ReadString());

                var toNode = _nodes[reader.ReadInt32()];
                var toNodeInfo = NodeInfo.Create(toNode);
                var toPortInfo = toNodeInfo.GetPortInfo(reader.ReadString());

                fromPortInfo.GetPort(fromNode).ConnectTo(toPortInfo.GetPort(toNode));
            }
        }
    }
}
