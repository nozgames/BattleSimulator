using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleSimulator.UI
{
    public class UINodePalette : MonoBehaviour
    {
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private GameObject _itemPrefab = null;
        [SerializeField] private GameObject _groupPrefab = null;

        private void Start()
        {
            // TODO: order by group and name
            foreach (var type in System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && typeof(AI.Node).IsAssignableFrom(t))))
            {
                var item = UINodePaletteItem.Create(type, _itemPrefab, _content);
                if (null == item)
                    continue;

                item.onDragBegin += OnItemDragBegin;
                item.onDragEnd += OnItemDragEnd;
                item.onDrag += OnItemDrag;
            }
        }

        private void OnItemDragBegin(UIListItem item, PointerEventData eventData)
        {
            GetComponentInParent<UIGraph>().BeginDrag(((UINodePaletteItem)item).nodeInfo, eventData);
        }

        private void OnItemDragEnd(UIListItem item, PointerEventData eventData)
        {
            GetComponentInParent<UIGraph>().EndDrag (((UINodePaletteItem)item).nodeInfo, eventData);
        }

        private void OnItemDrag(UIListItem item, PointerEventData eventData)
        {
            GetComponentInParent<UIGraph>().ContinueDrag(((UINodePaletteItem)item).nodeInfo, eventData);
        }
    }
}

