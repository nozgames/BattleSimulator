using System;
using UnityEngine;

namespace BattleSimulator.UI
{
    internal class AddNodeCommand : Command
    {
        private UIGraph _uigraph;
        private AI.Node _node;
        private UINode _uinode;
        private Vector2 _position;
        private Transform _parent;

        public UINode uinode => _uinode;

        public AddNodeCommand(UIGraph uigraph, AI.Node node, Vector2 position)
        {
            _uigraph = uigraph;
            _node = node;
            _position = position;
        }

        public AddNodeCommand(UIGraph uigraph, UINode uinode, Vector2 position)
        {
            _uigraph = uigraph;
            _node = uinode.node;
            _uinode = uinode;
            _position = position;
        }

        public override void OnExecute()
        {
            // Add the node to the graph first
            _uigraph.graph.AddNode(_node);

            // Now add it to the uigraph
            if(_uinode == null)
                _uinode = _uigraph.CreateNode(_node, _position);

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
