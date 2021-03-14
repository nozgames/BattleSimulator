using UnityEngine;
using BattleSimulator.Simulation;
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

        public UIGraph uigraph { get; private set; }
        public UIPort[] ports { get; private set; }
        public NodeInfo nodeInfo { get; private set; }
        public Vector2 position => _rect.anchoredPosition;

        public Abilities.Ability ability { get; private set; }

        public bool selected {
            get => _selected.activeSelf;
            set => _selected.SetActive(value);
        }

        public static UINode Create(UIGraph uigraph, Abilities.Ability ability, GameObject prefab, RectTransform parent, Vector2 position)
        {
            var nodeInfo = NodeInfo.Create(typeof(AbilityNodeWithTarget));
            var uinode = Create(uigraph, nodeInfo, prefab, parent, position);
            uinode.ability = ability;

            if (uinode._name != null)
                uinode._name.text = ability.name;

            return uinode;
        }

        public static UINode Create (UIGraph uigraph, NodeInfo nodeInfo, GameObject prefab, RectTransform parent, Vector2 position)
        {
            UINode uinode = null;
            if (nodeInfo.flags.HasFlag(NodeFlags.Compact))
                uinode = Instantiate(prefab, parent).GetComponent<UINode>();
            else
                uinode = Instantiate(prefab, parent).GetComponent<UINode>();

            uinode.uigraph = uigraph;
            uinode.GetComponent<RectTransform>().anchoredPosition = position;

            if(uinode._name != null)
                uinode._name.text = nodeInfo.name;

            uinode.nodeInfo = nodeInfo;
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
        }

        public void Move(Vector2 position) => MoveTo(_rect.anchoredPosition += position);

        public UIPort GetPort (PortInfo portInfo)
        {
            foreach (var uiport in ports)
                if (uiport.portInfo == portInfo)
                    return uiport;

            return null;
        }
    }
}
