using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace BattleSimulator.UI
{
    [RequireComponent(typeof(UILineRenderer))]
    class UIWireRenderer : MonoBehaviour
    {
        [SerializeField] private float _stemLength = 10.0f;
        [SerializeField] private Image _fromCap = null;
        [SerializeField] private Image _toCap = null;

        private RectTransform _rect;
        private UILineRenderer _line;
        private Vector2 _from;
        private Vector2 _to;
        private Color _fromColor;
        private Color _toColor;

        public Vector2 from {
            get => _from;
            set {
                _from = value;
                UpdateLine();
            }
        }

        public Color fromColor {
            get => _fromColor;
            set {
                _fromColor = value;
                UpdateLine();
            }
        }

        public Vector2 to {
            get => _to;
            set {
                _to = value;
                UpdateLine();
            }
        }

        public Color toColor {
            get => _toColor;
            set {
                _toColor = value;
                UpdateLine();
            }
        }

        private void Awake()
        {
            _line = GetComponent<UILineRenderer>();
            _rect = GetComponent<RectTransform>();
            _line.enabled = false;
        }

        private void OnEnable()
        {
            UpdateLine();
        }

        private void OnDisable()
        {
            _line.enabled = false;
        }

        private void UpdateLine()
        {
            if (!isActiveAndEnabled)
                return;

            _rect.anchoredPosition = (_from + _to) * 0.5f;
            _rect.sizeDelta = Vector2.Max(_from,_to) - Vector2.Min(_from,_to) + Vector2.one * 16;

            if (null != _fromCap)
            {
                _fromCap.color = _fromColor;
                _fromCap.GetComponent<RectTransform>().anchoredPosition = _from - _rect.anchoredPosition;
            }

            if (null != _toCap)
            {
                _toCap.color = _toColor;
                _toCap.GetComponent<RectTransform>().anchoredPosition = _to - _rect.anchoredPosition;
            }

            if (from == to)
            {
                _line.enabled = false;
                _line.Points = new Vector2[] { };
                return;
            }

            _line.enabled = true;
            _line.color = _fromColor;

            var stem = _rect.sizeDelta.magnitude * _stemLength;

            _line.Points = new Vector2[] {
                (_from - _rect.anchoredPosition),
                (_from - _rect.anchoredPosition) + new Vector2(stem, 0),
                (_to - _rect.anchoredPosition) - new Vector2(stem, 0),
                (_to - _rect.anchoredPosition)
            };         
        }

        public bool HitTest(Vector2 position, float threshold = 1.0f) =>
            _line.GetDistance(position) <= _line.LineThickness + threshold;
    }
}
