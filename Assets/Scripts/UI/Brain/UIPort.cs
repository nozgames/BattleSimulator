using UnityEngine;
using UnityEngine.UI;

using BattleSimulator.AI;
using System.Collections.Generic;

namespace BattleSimulator.UI
{
    public class UIPort : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _name = null;
        [SerializeField] private Image _icon = null;

        public UINode node { get; private set; }
        public List<UIWire> wires { get; private set; }
        public Port port { get; private set; }
        public PortInfo portInfo { get; private set; }

        public RectTransform connection => _icon.GetComponent<RectTransform>();

        public static UIPort Create (PortInfo portInfo, UINode uinode, GameObject prefab, RectTransform parent)
        {
            var uiport = Instantiate(prefab, parent).GetComponent<UIPort>();
            uiport.portInfo = portInfo;
            uiport.port = (Port)portInfo.property.GetValue(uinode.node);
            uiport.wires = new List<UIWire>();
            uiport.node = uinode;
            uiport._icon.color = UIManager.GetPortColor(uiport.port);
            uiport._name.text = portInfo.name;

            return uiport;
        }
    }
}
