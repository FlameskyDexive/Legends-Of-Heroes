using UnityEngine;
using UnityEngine.EventSystems;

namespace ET.Client
{
    /// <summary>
    /// UGUI 事件监听器：将 Unity EventTrigger 的常用事件聚合为简单的 delegate。
    /// </summary>
    public class EventTriggerListener : EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public delegate void VectorDelegate(GameObject go, Vector2 delta);

        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VectorDelegate onDrag;
        public VoidDelegate onDragOut;

        public static EventTriggerListener Get(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("EventTriggerListener_go_is_NULL");
                return null;
            }
            var listener = go.GetComponent<EventTriggerListener>();
            if (listener == null)
            {
                listener = go.AddComponent<EventTriggerListener>();
            }
            return listener;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(gameObject, eventData.delta);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            onDragOut?.Invoke(gameObject);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(gameObject);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            onDown?.Invoke(gameObject);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.Invoke(gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            onExit?.Invoke(gameObject);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            onUp?.Invoke(gameObject);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            onSelect?.Invoke(gameObject);
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelect?.Invoke(gameObject);
        }
    }
}
