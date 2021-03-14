using System;
using BattleSimulator.Simulation;
using UnityEngine;

namespace BattleSimulator.UI
{
    internal class AddNodeCommand : Command
    {
        private UIGraph _uigraph;
        private UINode _uinode;
        private NodeInfo _nodeInfo;
        private Vector2 _position;
        private Transform _parent;

        public AddNodeCommand(UIGraph uigraph, NodeInfo nodeInfo, Vector2 position)
        {
            _uigraph = uigraph;
            _position = position;
            _nodeInfo = nodeInfo;
        }

        public AddNodeCommand(UIGraph uigraph, UINode uinode, Vector2 position)
        {
            _uigraph = uigraph;
            _uinode = uinode;
            _position = position;
            _nodeInfo = uinode.nodeInfo;
        }

        public override void OnExecute()
        {
            // Now add it to the uigraph
            if(_uinode == null)
                _uinode = _uigraph.CreateNode(_nodeInfo, _position);

            _uigraph.SelectNode(_uinode);

            _parent = _uinode.transform.parent;
        }

        public override void OnRedo()
        {
            _uinode.transform.SetParent(_parent);
            _uigraph.SelectNode(_uinode);
        }

        public override void OnUndo()
        {
            _uigraph.UnselectNode(_uinode);
            _uinode.transform.SetParent(_uigraph.trash);
        }

        public override void Dispose()
        {
            if (!isExecuted)
                UnityEngine.Object.Destroy(_uinode.gameObject);
        }
    }
}
