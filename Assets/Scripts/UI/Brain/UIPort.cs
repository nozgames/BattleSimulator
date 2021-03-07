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

        public UINode uinode { get; private set; }
        public List<UIWire> wires { get; private set; }
        public PortInfo portInfo { get; private set; }

        public RectTransform connection => _icon.GetComponent<RectTransform>();

        public static UIPort Create (PortInfo portInfo, UINode uinode, GameObject prefab, RectTransform parent)
        {
            var uiport = Instantiate(prefab, parent).GetComponent<UIPort>();
            uiport.portInfo = portInfo;
            uiport.wires = new List<UIWire>();
            uiport.uinode = uinode;
            uiport._icon.color = UIManager.GetPortColor(uiport.portInfo);
            uiport._name.text = portInfo.name;

            return uiport;
        }

        public bool CanConnectTo(UIPort uiport)
        {
            if (uiport == null || uiport == this)
                return false;

            if (uiport.uinode == uinode)
                return false;

            if (uiport.portInfo.flow == portInfo.flow)
                return false;

            return true;
        }
    }
}
