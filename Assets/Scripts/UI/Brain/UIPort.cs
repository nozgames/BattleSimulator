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

        public bool isInput => portInfo.flow == PortFlow.Input;
        public bool isOutput => portInfo.flow == PortFlow.Output;

        public RectTransform connection => _icon.GetComponent<RectTransform>();

        public Vector2 position => RectTransformUtility.CalculateRelativeRectTransformBounds(uinode.transform.parent, connection).center;

        public static UIPort Create (PortInfo portInfo, UINode uinode, GameObject prefab, RectTransform parent)
        {
            var uiport = Instantiate(prefab, parent).GetComponent<UIPort>();
            uiport.portInfo = portInfo;
            uiport.wires = new List<UIWire>();
            uiport.uinode = uinode;
            uiport._icon.color = UIManager.GetPortColor(uiport.portInfo);

            if(uiport._name != null)
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

            // Unit ports must connect to unit ports
            var type0 = portInfo.type;
            var type1 = uiport.portInfo.type;
            if (type0 != type1 && (type0 == typeof(UnitOutputPort) || type0 == typeof(UnitInputPort)) != (type1 == typeof(UnitOutputPort) || type1 == typeof(UnitInputPort)))
                return false;

            return true;
        }
    }
}
