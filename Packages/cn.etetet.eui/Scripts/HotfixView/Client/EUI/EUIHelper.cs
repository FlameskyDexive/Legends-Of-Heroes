using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ET.Client
{
    public static class EUIHelper
    {
        public static void SetText(this Text label, string content)
        {
            if (null == label)
            {
                Log.Error("label is null");
                return;
            }
            label.text = content;
        }

        public static void SetVisibleWithScale(this UIBehaviour uiBehaviour, bool isVisible)
        {
            if (null == uiBehaviour || null == uiBehaviour.gameObject)
            {
                Log.Error("uiBehaviour or gameObject is null!");
                return;
            }
            if (uiBehaviour.gameObject.activeSelf == isVisible)
            {
                return;
            }
            uiBehaviour.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
        }

        public static void SetVisible(this UIBehaviour uiBehaviour, bool isVisible)
        {
            if (null == uiBehaviour || null == uiBehaviour.gameObject)
            {
                Log.Error("uiBehaviour or gameObject is null!");
                return;
            }
            if (uiBehaviour.gameObject.activeSelf == isVisible)
            {
                return;
            }
            uiBehaviour.gameObject.SetActive(isVisible);
        }

        public static void SetVisibleWithScale(this Transform transform, bool isVisible)
        {
            if (null == transform || null == transform.gameObject)
            {
                Log.Error("transform or gameObject is null!");
                return;
            }
            transform.localScale = isVisible ? Vector3.one : Vector3.zero;
        }

        public static void SetVisible(this Transform transform, bool isVisible)
        {
            if (null == transform || null == transform.gameObject)
            {
                Log.Error("transform or gameObject is null!");
                return;
            }
            if (transform.gameObject.activeSelf == isVisible)
            {
                return;
            }
            transform.gameObject.SetActive(isVisible);
        }

        public static void SetTogglesInteractable(this ToggleGroup toggleGroup, bool isEnable)
        {
            var toggles = toggleGroup.transform.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].interactable = isEnable;
            }
        }

        public static (int, Toggle) GetSelectedToggle(this ToggleGroup toggleGroup)
        {
            var togglesList = toggleGroup.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < togglesList.Length; i++)
            {
                if (togglesList[i].isOn)
                {
                    return (i, togglesList[i]);
                }
            }
            Log.Error("none Toggle is Selected");
            return (-1, null);
        }

        public static void SetToggleSelected(this ToggleGroup toggleGroup, int index)
        {
            var togglesList = toggleGroup.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < togglesList.Length; i++)
            {
                if (i != index)
                {
                    continue;
                }
                togglesList[i].IsSelected(true);
            }
        }

        public static void IsSelected(this Toggle toggle, bool isSelected)
        {
            toggle.isOn = isSelected;
            toggle.onValueChanged?.Invoke(isSelected);
        }

        public static void RemoveUIScrollItems<K, T>(this K self, ref Dictionary<int, EntityRef<T>> dictionary)
            where K : Entity, IUILogic
            where T : Entity, IAwake, IUIScrollItem<T>
        {
            if (dictionary == null)
            {
                return;
            }
            foreach (var item in dictionary)
            {
                T value = item.Value;
                value?.Dispose();
            }
            dictionary.Clear();
            dictionary = null;
        }

        public static void AddListenerAsyncWithId(this Button button, Scene rootScene, Func<int, ETTask> action, int id)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action(id).Coroutine());
        }

        public static void AddListenerAsync(this Button button, Scene rootScene, Func<ETTask> action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action().Coroutine());
        }

        public static void AddListener(this Toggle toggle, Scene rootScene, UnityAction<bool> selectEventHandler)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(selectEventHandler);
        }

        public static void AddListener(this Button button, Scene rootScene, UnityAction clickEventHandler)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(clickEventHandler);
        }

        public static void AddListenerWithId(this Button button, Scene rootScene, Action<int> clickEventHandler, int id)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clickEventHandler(id));
        }

        public static void AddListenerWithId(this Button button, Scene rootScene, Action<long> clickEventHandler, long id)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clickEventHandler(id));
        }

        public static void AddListenerWithParam<T>(this Button button, Scene rootScene, Action<T> clickEventHandler, T param)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clickEventHandler(param));
        }

        public static void AddListenerWithParam<T, A>(this Button button, Scene rootScene, Action<T, A> clickEventHandler, T param1, A param2)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clickEventHandler(param1, param2));
        }

        public static void AddListener(this ToggleGroup toggleGroup, Scene rootScene, UnityAction<int> selectEventHandler)
        {
            var togglesList = toggleGroup.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < togglesList.Length; i++)
            {
                int index = i;
                togglesList[i].AddListener(rootScene, isOn =>
                {
                    if (isOn)
                    {
                        selectEventHandler(index);
                    }
                });
            }
        }

        public static void RegisterEvent(this EventTrigger trigger, Scene rootScene, EventTriggerType eventType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = null;
            foreach (EventTrigger.Entry existingEntry in trigger.triggers)
            {
                if (existingEntry.eventID == eventType)
                {
                    entry = existingEntry;
                    break;
                }
            }
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = eventType };
            }
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
    }
}
