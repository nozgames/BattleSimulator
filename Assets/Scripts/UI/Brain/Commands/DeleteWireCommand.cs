
using UnityEngine;

namespace BattleSimulator.UI
{
    class DeleteWireCommand : Command
    {
        private UIWire _uiwire;
        private Transform _undoParent;

        public DeleteWireCommand(UIWire uiwire)
        {
            _uiwire = uiwire;
            _undoParent = _uiwire.transform.parent;
        }

        public override void OnExecute()
        {
            // Move to the trash
            _uiwire.transform.SetParent(_uiwire.from.uinode.uigraph.trash);
            _uiwire.from.wires.Remove(_uiwire);
            _uiwire.to.wires.Remove(_uiwire);
        }

        public override void OnRedo() => OnExecute();

        public override void OnUndo()
        {
            _uiwire.transform.SetParent(_undoParent);
            _uiwire.from.wires.Add(_uiwire);
            _uiwire.to.wires.Add(_uiwire);
        }
    }
}
