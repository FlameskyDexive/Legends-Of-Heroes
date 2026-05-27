using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 带点击间隔的 UI 按钮：点击后短时间内不可再次触发。
    /// </summary>
    [AddComponentMenu("UI/IntervalButton")]
    public class IntervalButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
    {
        [Tooltip("How long must pointer be down on this object to trigger a interval press")]
        [FormerlySerializedAs("onClick")]
        private IntervalButtonClickedEvent m_OnClick = new IntervalButtonClickedEvent();

        public bool isGreyImage = false;
        public bool isInterval = false;
        public float interval = 1.8f;
        private float temInterval = 1.8f;
        private bool canInvoke = true;

        public IntervalButtonClickedEvent onClick
        {
            get => this.m_OnClick;
            set => this.m_OnClick = value;
        }

        private void Update()
        {
            if (!this.canInvoke)
            {
                this.temInterval -= Time.deltaTime;
                if (this.temInterval <= 0)
                {
                    this.canInvoke = true;
                    var img = this.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = Color.white;
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.isInterval && this.canInvoke)
            {
                this.temInterval = this.interval;
                onClick.Invoke();
                if (this.isGreyImage)
                {
                    var img = this.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = Color.grey;
                    }
                }
                this.canInvoke = false;
            }
        }

        [Serializable]
        public class IntervalButtonClickedEvent : UnityEvent
        {
        }
    }
}
