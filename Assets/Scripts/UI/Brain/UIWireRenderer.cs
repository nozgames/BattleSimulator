﻿using UnityEngine;
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
        }

        private void OnEnable()
        {
            UpdateLine();
        }

        private void UpdateLine()
        {
            if (!isActiveAndEnabled)
                return;

            _rect.anchoredPosition = _from;

            if (null != _fromCap)
            {
                _fromCap.color = _fromColor;
                _fromCap.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            if (null != _toCap)
            {
                _toCap.color = _toColor;
                _toCap.GetComponent<RectTransform>().anchoredPosition = _to - _from;
            }

            if (from == to)
            {
                _line.Points = new Vector2[] { };
                
                return;
            }

            _line.color = _fromColor;

            var stem = (_to - _from).magnitude * _stemLength;

            _line.Points = new Vector2[] {
                Vector2.zero,
                new Vector2(stem, 0),
                (_to - _from) - new Vector2(stem, 0),
                (_to - _from)
            };         
        }
    }
}