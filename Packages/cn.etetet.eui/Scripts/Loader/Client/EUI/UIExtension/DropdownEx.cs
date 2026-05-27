using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 扩展 Dropdown：每次 OnPointerClick 重置 Toggle 状态；可选始终回调（m_AlwaysCallback）。
    /// </summary>
    public class DropdownEx : Dropdown
    {
        public bool m_AlwaysCallback = false;

        public new void Show()
        {
            base.Show();
            var toggleRoot = transform.Find("Dropdown List/Viewport/Content");
            if (toggleRoot == null)
            {
                return;
            }
            var toggleList = toggleRoot.GetComponentsInChildren<Toggle>(false);
            for (int i = 0; i < toggleList.Length; i++)
            {
                Toggle temp = toggleList[i];
                temp.onValueChanged.RemoveAllListeners();
                temp.isOn = false;
                temp.onValueChanged.AddListener(_ => OnSelectItemEx(temp));
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            Show();
        }

        private void OnSelectItemEx(Toggle toggle)
        {
            if (!toggle.isOn)
            {
                toggle.isOn = true;
                return;
            }
            int selectedIndex = -1;
            var tr = toggle.transform;
            var parent = tr.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == tr)
                {
                    selectedIndex = i - 1;
                    break;
                }
            }
            if (selectedIndex < 0)
            {
                return;
            }
            if (value == selectedIndex && m_AlwaysCallback)
            {
                onValueChanged.Invoke(value);
            }
            else
            {
                value = selectedIndex;
            }
            Hide();
        }
    }
}
