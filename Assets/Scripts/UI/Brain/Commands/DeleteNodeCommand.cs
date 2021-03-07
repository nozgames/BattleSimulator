using System;
using UnityEngine;

namespace BattleSimulator.UI
{
    internal class DeleteNodeCommand : Command
    {
        private UINode _uinode;
        private Transform _parent;

        public UINode uinode => _uinode;

        public DeleteNodeCommand(UINode uinode)
        {
            _uinode = uinode;
            _parent = _uinode.transform.parent;
        }

        public override void OnExecute()
        {
            _uinode.transform.SetParent(_uinode.uigraph.trash);
            _uinode.uigraph.nodes.Remove(_uinode);
        }

        public override void OnRedo() => OnExecute();

        public override void OnUndo()
        {
            _uinode.transform.SetParent(_parent);
            _uinode.uigraph.nodes.Add(_uinode);
        }

        public override void Dispose()
        {
            if (isExecuted)
                UnityEngine.Object.Destroy(_uinode.gameObject);
        }
    }
}
