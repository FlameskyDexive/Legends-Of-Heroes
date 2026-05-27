using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 输入扩展：判断 Touch / 鼠标按下时是否打在了 UI。
    /// 用于摇杆 / TouchPad 等避免与 UI 击穿冲突的场景。
    /// </summary>
    public static class InputExtension
    {
        /// <summary>
        /// 判断 Touch 按下时是否打到了 UI 组件。
        /// filter 必须设定非空默认值，因为 string.StartsWith("") 永远返回 true。
        /// </summary>
        public static bool IsRaycastUI(this Touch touch, string filter = "#")
        {
            return Raycast(touch.position, filter);
        }

        /// <summary>
        /// 判断鼠标按下时是否打到了 UI 组件。
        /// </summary>
        public static bool IsMouseRaycastUI(string filter = "#")
        {
            return Raycast(Input.mousePosition, filter);
        }

        /// <summary>
        /// 执行射线检测确认是否打到了 UI。EventSystem 用全限定名以避开 ET.EventSystem 命名冲突。
        /// </summary>
        /// <param name="position">Touch 或者光标所在的位置</param>
        /// <param name="filterPrefix">希望忽略的 UI 节点名前缀</param>
        private static bool Raycast(Vector2 position, string filterPrefix)
        {
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (!eventSystem && !eventSystem.IsPointerOverGameObject())
            {
                return false;
            }
            var data = new PointerEventData(eventSystem)
            {
                pressPosition = position,
                position = position
            };
            var list = new List<RaycastResult>();
            eventSystem.RaycastAll(data, list);
            return list.Count > 0
                && list[0].module is GraphicRaycaster
                && !list[0].gameObject.name.StartsWith(filterPrefix);
        }
    }
}
