using UnityEngine;
using UnityEngine.UI;

using BattleSimulator.AI;

namespace BattleSimulator.UI
{
    public class UIWire : MonoBehaviour
    {
        private RectTransform _rect;
        private UIWireRenderer _renderer;
        private UIPort _from;
        private UIPort _to;

        public Wire wire { get; private set; }
        
        public UIPort from {
            get => _from;
            set {
                _from = value;
                UpdateRenderer();
            }
        }

        public UIPort to {
            get => _to;
            set {
                _to = value;
                UpdateRenderer();
            }
        }

        public static UIWire Create (Wire wire, GameObject prefab, RectTransform parent, UIPort from, UIPort to)
        {
            var uiwire = Instantiate(prefab, parent).GetComponent<UIWire>();
            uiwire.from = from;
            uiwire.to = to;
            uiwire.wire = wire;

            from.wires.Add(uiwire);
            to.wires.Add(uiwire);

            return uiwire;
        }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _renderer = GetComponent<UIWireRenderer>();
        }

        private void OnEnable()
        {
            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            if (!isActiveAndEnabled)
                return;

            if(_from == null || _to == null)
            {
                _renderer.enabled = false;
                return;
            }

            _renderer.from = RectTransformUtility.CalculateRelativeRectTransformBounds(_rect.parent.parent, _from.connection).center;
            _renderer.to = RectTransformUtility.CalculateRelativeRectTransformBounds(_rect.parent.parent, _to.connection).center;
            _renderer.fromColor = UIManager.GetPortColor(_from.port);
            _renderer.toColor = UIManager.GetPortColor(_to.port);
            _renderer.enabled = true;
        }
    }
}
