using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BattleSimulator.Extensions;

namespace BattleSimulator.AI
{
    public class Graph
    {
        private const int FileVersion = 1;

        private List<Node> _nodes = new List<Node>();

        // TODO: This could be build from the node list afer loading a graph
        private List<ActionNode> _actions = new List<ActionNode>();

        public List<Node> nodes => _nodes;

        // TODO: what parameters
        public AI.Target Execute(Context context)
        {
            foreach (var action in _actions)
            {
                action.Execute(context);
            }

            var bestPriority = Priority.none;
            var bestAction = (ActionNode)null;
            foreach (var action in _actions)
            {
                action.Execute(context);

                if (action.priority > bestPriority)
                {
                    bestAction = action;
                    bestPriority = action.priority;
                }
            }

            if (bestAction != null)
            {
                // TODO: perform the action somehow..  Probably attach some data to the action that 
                //       the caller can use to determine what action to perform.  Also need to trigger the cooldown as well.

                //bestAction.Perform();
            }

            if (bestAction is ActionNodeWithTarget actionWithTarget)
                return actionWithTarget.target;

            return null;
        }

        /// <summary>
        /// Add a node to the graph
        /// </summary>
        /// <param name="node">Node to add</param>
        public void AddNode(Node node)
        {
            if (_nodes.Contains(node))
                return;

            _nodes.Add(node);

            if (node is ActionNode actionNode)
                _actions.Add(actionNode);
        }

        /// <summary>
        /// Remove a node from the graph
        /// </summary>
        /// <param name="node">Node to remove</param>
        public void RemoveNode(Node node)
        {
            _nodes.Remove(node);

            if (node is ActionNode actionNode)
                _actions.Remove(actionNode);
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
