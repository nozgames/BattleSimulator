
using UnityEngine;

namespace BattleSimulator.UI
{
    class AddWireCommand : Command
    {
        private UIPort _from;
        private UIPort _to;
        private UIWire _uiwire;
        private Transform _parent;

        public AddWireCommand (UIPort from, UIPort to)
        {
            _from = from;
            _to = to;
        }

        public override void OnExecute()
        {
            _uiwire = _from.uinode.uigraph.CreateWire(_from, _to);
        }

        public override void OnRedo()
        {
            _uiwire.transform.SetParent(_parent);
            _from.wires.Add(_uiwire);
            _to.wires.Add(_uiwire);
        }

        public override void OnUndo()
        {
            _parent = _uiwire.transform.parent;
            _from.wires.Remove(_uiwire);
            _to.wires.Remove(_uiwire);
            _uiwire.transform.SetParent(_from.uinode.uigraph.trash);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!isExecuted)
                UnityEngine.Object.Destroy(_uiwire);
        }
    }
}
