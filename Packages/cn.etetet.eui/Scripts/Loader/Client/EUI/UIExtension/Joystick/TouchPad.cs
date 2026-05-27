using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Client
{
    /// <summary>
    /// 触控板：屏幕右半屏接收非 UI 的触摸滑动，常用于第三人称视角控制。
    /// </summary>
    public class TouchPad : MonoBehaviour
    {
        /// <summary>
        /// 以此字符开头的 UI 节点不会阻碍 TouchPad 拖拽。
        /// </summary>
        public string filterPrefix = "#";

        public TouchPadEvent OnTouchPadValueChanged = new TouchPadEvent();

        [System.Serializable]
        public class TouchPadEvent : UnityEvent<Vector2>
        {
        }

        private readonly List<Touch> fingerIds = new List<Touch>();
        private bool canmove;

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                foreach (var item in Input.touches)
                {
                    int index = fingerIds.FindIndex(touch => touch.fingerId == item.fingerId);
                    if (index == -1)
                    {
                        // 首次按下 + 没打到 UI + 在屏幕右半部
                        if (item.phase == TouchPhase.Began && item.position.x > Screen.width * 0.5f && !item.IsRaycastUI(filterPrefix))
                        {
                            fingerIds.Add(item);
                        }
                    }
                    else
                    {
                        if (item.phase == TouchPhase.Ended || item.phase == TouchPhase.Canceled)
                        {
                            fingerIds.RemoveAt(index);
                        }
                        else
                        {
                            fingerIds[index] = item;
                        }
                    }
                }
                foreach (var item in fingerIds)
                {
                    if (item.phase == TouchPhase.Moved)
                    {
                        OnTouchPadValueChanged.Invoke(item.deltaPosition);
                    }
                }
            }

#if UNITY_EDITOR
            // 编辑器内用鼠标右键模拟 TouchPad，便于调试。
            if (Input.GetMouseButtonDown(1) && !InputExtension.IsMouseRaycastUI("#"))
            {
                canmove = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                canmove = false;
            }
            if (Input.GetMouseButton(1) && canmove)
            {
                var h = Input.GetAxis("Mouse X");
                var v = Input.GetAxis("Mouse Y");
                OnTouchPadValueChanged.Invoke(new Vector2(h, v));
            }
#endif
        }
    }
}
