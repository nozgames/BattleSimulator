using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator.UI
{
    class UINodePaletteItem : UIListItem
    {
        [SerializeField] private TMPro.TextMeshProUGUI _name = null;
        [SerializeField] private Image _icon = null;

        public AI.NodeInfo nodeInfo { get; private set; }

        public static UINodePaletteItem Create (Type type, GameObject prefab, RectTransform parent)
        {
            var nodeInfo = AI.NodeInfo.Create(type);
            if (nodeInfo.flags.HasFlag(AI.NodeFlags.Hidden))
                return null;
            
            var item = Instantiate(prefab, parent).GetComponent<UINodePaletteItem>();
            item.nodeInfo = nodeInfo;
            item._name.text = nodeInfo.name;
            return item;
        }
    }
}
