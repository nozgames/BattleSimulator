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
        private bool _dirty = true;
        
        public UIPort from {
            get => _from;
            set {
                _from = value;
                _dirty = true;
            }
        }

        public UIPort to {
            get => _to;
            set {
                _to = value;
                _dirty = true;
            }
        }

        public static UIWire Create (UIPort from, UIPort to, GameObject prefab, RectTransform parent)
        {
            var uiwire = Instantiate(prefab, parent).GetComponent<UIWire>();
            uiwire.from = from;
            uiwire.to = to;
            uiwire._dirty = true;

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
            _dirty = true;
        }

        private void LateUpdate()
        {
            if (_from == null || _to == null)
            {
                _renderer.enabled = false;
                return;
            }

            var fromPosition = (Vector2)RectTransformUtility.CalculateRelativeRectTransformBounds(_rect.parent.parent, _from.connection).center;
            var toPosition = (Vector2)RectTransformUtility.CalculateRelativeRectTransformBounds(_rect.parent.parent, _to.connection).center;

            if (_dirty || fromPosition != _renderer.from || toPosition != _renderer.to)
                UpdateRenderer(fromPosition, toPosition);
        }

        private void UpdateRenderer(Vector2 fromPosition, Vector2 toPosition)
        {
            _dirty = false;
            _renderer.from = fromPosition;
            _renderer.to = toPosition;
            _renderer.fromColor = UIManager.GetPortColor(_from.portInfo);
            _renderer.toColor = UIManager.GetPortColor(_to.portInfo);
            _renderer.enabled = true;
        }
    }
}
