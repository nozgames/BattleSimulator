using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BattleSimulator.Extensions;

#if false

- Register class
- Compiling graph will use registers to values from ports


- Preventing double execution of a node
    - If within a context we flag the node as executed then it wouldnt execute again
    - This does not work with target finders which may be referenced with a different stack
    - Target finder node would need its own context depending on the unit that was calling it
        - How to efficiently store state for each target combination?
        - Would need to store the answer for each unit 
            - Could use a native array of priorities

    - Every ouput port has a state per unit basically 
    - If we assign each port a register that has one value per unit?
        - 1024 units / 256 ports = 262144 registers * 8 bytes per register = 2 MB 

    - IF all nodes remember which unit they were last executed with they can early out if that unit is the same since the answer would be the same

#endif


namespace BattleSimulator.AI
{
    public abstract class Graph
    {
        private const int FileVersion = 1;

        private List<Node> _nodes = new List<Node>();

        public List<Node> nodes => _nodes;

        public abstract void Compile();

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

                    switch (property.type)
                    {
                        case NodePropertyType.Float:
                            writer.Write((float)property.propertyInfo.GetValue(node));
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

        public void Load (string path)
        {
            using (var file = File.OpenRead(path))
            using (var reader = new BinaryReader(file))
                Load(reader);
        }

        private void Load (BinaryReader reader)
        {
            if (!reader.ReadFourCC('B', 'B', 'G', 'R'))
                throw new InvalidDataException("not a graph");

            var version = reader.ReadInt32();

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
