using System;
using UnityEngine;
using UnityEngine.UI;
using BattleSimulator.Abilities;
using BattleSimulator.Simulation;

namespace BattleSimulator.UI
{
    public class UINodePaletteItem : UIListItem
    {
        [SerializeField] private TMPro.TextMeshProUGUI _name = null;
        [SerializeField] private Image _icon = null;

        public NodeInfo nodeInfo { get; private set; }

        public Ability ability { get; private set; }

        public static UINodePaletteItem Create (Type type, GameObject prefab, RectTransform parent)
        {
            var nodeInfo = NodeInfo.Create(type);
            if (nodeInfo.flags.HasFlag(NodeFlags.Hidden))
                return null;
            
            var item = Instantiate(prefab, parent).GetComponent<UINodePaletteItem>();
            item.nodeInfo = nodeInfo;
            item._name.text = nodeInfo.name;
            return item;
        }

        /// <summary>
        /// Create a node palette item from an ability
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static UINodePaletteItem Create(Abilities.Ability ability, GameObject prefab, RectTransform parent)
        {
            var nodeInfo = NodeInfo.Create(typeof(Simulation.AbilityNodeWithTarget));
            var item = Instantiate(prefab, parent).GetComponent<UINodePaletteItem>();
            item.nodeInfo = nodeInfo;
            item._name.text = ability.name;
            item.ability = ability;
            return item;
        }
    }
}
