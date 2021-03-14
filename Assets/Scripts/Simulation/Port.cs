using System;
using System.Collections.Generic;

namespace BattleSimulator.Simulation
{
    public class Port
    {
        private PortInfo _info = null;

        /// <summary>
        /// Returns the number of wires in the given port
        /// </summary>
        public int wireCount => wires.Count;

        public Node node { get; private set; }

        public PortFlow flow { get; private set; }        

        public List<Wire> wires { get; private set; }

        public PortInfo info {
            get {
                if(_info == null)
                    _info = NodeInfo.Create(node).GetPortInfo(this);

                return _info;
            }
        }

        /// <summary>
        /// Construct the port with the node that the port belongs to
        /// </summary>
        /// <param name="node">Node the port belongs to</param>
        public Port (Node node, PortFlow flow)
        {
            this.node = node;
            this.flow = flow;
            wires = new List<Wire>(1);
        }

        /// <summary>
        /// Return the port connected to this port through the given wire.  If the wire is not connect
        /// to the given port then null will be returned.
        /// </summary>
        /// <param name="wire">Wire to get connected port through</param>
        /// <returns>Port connected to the port through the given wire</returns>
        public Port GetConnectedPort (Wire wire)
        {
            if (wire.from == this)
                return wire.to;
            else if (wire.to == this)
                return wire.from;

            return null;
        }

        /// <summary>
        /// Returns true if the given port is connected to this port
        /// </summary>
        /// <param name="port">Port to test</param>
        /// <returns>True if the two ports are connected</returns>
        public bool IsConnectedTo (Port port)
        {
            if (port == null)
                return false;

            if (port == this)
                return false;

            foreach (var wire in wires)
                if (wire.from == port || wire.to == port)
                    return true;

            return false;
        }

        /// <summary>
        /// Connect the the port to the given port via a wire
        /// </summary>
        /// <param name="port">Port to connect to</param>
        public Wire ConnectTo (Port port)
        {
            if (null == port)
                throw new ArgumentNullException("port");

            if (flow == port.flow)
                throw new InvalidOperationException($"cannot connect {flow} port {port.flow} port");

            // Make sure the wire isnt already connected
            if (IsConnectedTo(port))
                throw new InvalidOperationException("port is already connected");

            // Add a new wire to both ports
            var wire = (flow == PortFlow.Output ?
                new Wire(this as OutputPort, port as InputPort) :
                new Wire(port as OutputPort, this as InputPort));

            wires.Add(wire);
            port.wires.Add(wire);

            return wire;
        }

        private void Execute (Context context)
        {
            if(flow == PortFlow.Input)
            {
                foreach (var wire in wires)
                    wire.from.Execute(context);                
            }
            else
            {
                // Execute the node first which should set the value in the port
                // TODO: need way to prevent unnecessary execution of nodes (compiler maybe)
                node.Execute(context);
            }
        }        
    }
}
