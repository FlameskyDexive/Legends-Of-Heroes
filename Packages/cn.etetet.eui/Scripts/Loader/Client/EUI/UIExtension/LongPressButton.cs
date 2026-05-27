using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 长按按钮：超过 durationThreshold 触发 onLongPress；正常按下抬起触发 onClick。
    /// 可设置 isLooped + interval 让长按周期性触发。
    /// </summary>
    [AddComponentMenu("UI/LongPressButton")]
    public class LongPressButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
    {
        [Tooltip("How long must pointer be down on this object to trigger a long press")]
        [FormerlySerializedAs("onClick")]
        private LongPressButtonClickedEvent m_OnClick = new LongPressButtonClickedEvent();

        private LongPressButtonLongPressedEvent m_OnLongPress = new LongPressButtonLongPressedEvent();
        public float durationThreshold = 1.0f;
        public bool isLooped = true;
        public float interval = 0.2f;
        private bool restart = true;
        private int curTimes;

        private bool isPointerDown;
        private bool longPressTriggered;
        private float timePressStarted;

        public LongPressButtonClickedEvent onClick
        {
            get => this.m_OnClick;
            set => this.m_OnClick = value;
        }

        public LongPressButtonLongPressedEvent onLongPress
        {
            get => this.m_OnLongPress;
            set => this.m_OnLongPress = value;
        }

        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold + this.curTimes * this.interval)
                {
                    if (!this.isLooped)
                    {
                        longPressTriggered = true;
                    }
                    this.restart = true;
                    if (this.restart)
                    {
                        onLongPress.Invoke();
                        this.curTimes++;
                        this.restart = false;
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            restart = true;
            curTimes = 0;
            longPressTriggered = false;
            isPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            restart = true;
            curTimes = 0;
            longPressTriggered = false;
            isPointerDown = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!longPressTriggered)
            {
                onClick.Invoke();
            }
        }

        [Serializable]
        public class LongPressButtonClickedEvent : UnityEvent
        {
        }

        [Serializable]
        public class LongPressButtonLongPressedEvent : UnityEvent
        {
        }
    }
}
