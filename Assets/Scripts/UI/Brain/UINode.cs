using UnityEngine;
using BattleSimulator.AI;
using System.Linq;

namespace BattleSimulator.UI
{
    public class UINode : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _name = null;
        [SerializeField] private GameObject _inputPrefab = null;
        [SerializeField] private GameObject _outputPrefab = null;
        [SerializeField] private RectTransform _inputsTransform = null;
        [SerializeField] private RectTransform _outputsTransform = null;
        [SerializeField] private GameObject _selected = null;

        private RectTransform _rect;

        public UIPort[] ports { get; private set; }
        public NodeInfo nodeInfo { get; private set; }
        public Node node { get; private set; }

        public bool selected {
            get => _selected.activeSelf;
            set => _selected.SetActive(value);
        }

        public static UINode Create (Node node, GameObject prefab, RectTransform parent, Vector2 position)
        {
            var nodeInfo = NodeInfo.Create(node);
            if (null == nodeInfo)
                return null;

            UINode uinode = null;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compact))
                uinode = Instantiate(prefab, parent).GetComponent<UINode>();
            else
                uinode = Instantiate(prefab, parent).GetComponent<UINode>();

            uinode.GetComponent<RectTransform>().anchoredPosition = position;
            uinode._name.text = nodeInfo.name;
            uinode.nodeInfo = nodeInfo;
            uinode.node = node;
            uinode.ports = nodeInfo.ports.Select(port =>
                UIPort.Create(
                    port,
                    uinode,
                    port.flow == PortFlow.Input ? uinode._inputPrefab : uinode._outputPrefab,
                    port.flow == PortFlow.Input ? uinode._inputsTransform : uinode._outputsTransform)).ToArray();

            return uinode;
        }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void MoveTo (Vector2 position)
        {
            _rect.anchoredPosition = position;

            foreach(var port in ports)
                foreach (var wire in port.wires)
                    wire.UpdateRenderer();
        }

        public void Move(Vector2 position) => MoveTo(_rect.anchoredPosition += position);
    }
}
