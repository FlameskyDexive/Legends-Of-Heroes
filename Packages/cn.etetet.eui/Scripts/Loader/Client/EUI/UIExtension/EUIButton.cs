using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// EUI 按钮：支持点击间隔、按压缩放、长按事件。
    /// </summary>
    public class EUIButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        public enum AudioClickEnum
        {
            None = 0,
            Confirm = 1,
            Cancel = 2,
            GoHome = 3,
            GoExplore = 4,
        }

        public AudioClickEnum m_audioClick = AudioClickEnum.None;

        [Tooltip("是否需要点击间隔")]
        public bool m_isNeedInterval = false;
        public float m_IntervalTime = 0;
        public bool m_isClick = false;

        [Tooltip("是否需要按压缩放")]
        public bool m_isNeedScale = false;
        private readonly Vector3 m_Forme = Vector3.one;
        public Vector3 m_To = new Vector3(0.8f, 0.8f, 0.8f);

        private bool m_isPress;
        public bool IsPress
        {
            get => m_isPress;
            private set => m_isPress = value;
        }

        // 长按相关
        private bool m_isStartPress;
        private float m_curPointDownTime;
        private bool m_longPressTrigger;
        [Tooltip("触发长按事件时间间隔（秒）")]
        public float m_longPressTime = 0.5f;

        public ButtonPressEvent m_onLongPress { get; set; }
        public ButtonPointerClickEvent m_onPointerClick { get; set; }

        public ButtonPressEvent OnLongPress
        {
            get => m_onLongPress ??= new ButtonPressEvent();
            set => m_onLongPress = value;
        }

        public ButtonPointerClickEvent OnPointClick
        {
            get => m_onPointerClick ??= new ButtonPointerClickEvent();
            set => m_onPointerClick = value;
        }

        protected override void Awake()
        {
            base.Awake();
            m_onLongPress ??= new ButtonPressEvent();
        }

        protected override void Start()
        {
            base.Start();
            if (this.onClick != null)
            {
                this.onClick.AddListener(ClickCallBack);
            }
        }

        private void Update()
        {
            if (m_isNeedInterval && m_isClick)
            {
                m_IntervalTime += Time.deltaTime;
                if (m_IntervalTime > 0.5f)
                {
                    m_IntervalTime = 0;
                    this.interactable = true;
                    m_isClick = false;
                }
            }
            CheckIsLongPress();
        }

        protected override void OnDestroy()
        {
            if (this.onClick != null)
            {
                this.onClick.RemoveAllListeners();
            }
            base.OnDestroy();
        }

        public void SetPress(bool isPress)
        {
            IsPress = isPress;
            if (isPress)
            {
                image.sprite = spriteState.pressedSprite;
            }
            else
            {
                image.sprite = spriteState.disabledSprite;
            }
        }

        public void SetSelectionStateNormal()
        {
            DoStateTransition(SelectionState.Normal, false);
        }

        private void ClickCallBack()
        {
            if (m_isNeedInterval)
            {
                m_isClick = true;
                this.interactable = false;
            }
        }

        private void CheckIsLongPress()
        {
            if (m_isStartPress && !m_longPressTrigger)
            {
                if (Time.time > m_curPointDownTime + m_longPressTime)
                {
                    m_longPressTrigger = true;
                    m_isStartPress = false;
                    m_onLongPress?.Invoke(true);
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            m_curPointDownTime = Time.time;
            m_isStartPress = true;
            m_longPressTrigger = false;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            m_isStartPress = false;
            if (m_longPressTrigger)
            {
                m_longPressTrigger = false;
                m_onLongPress?.Invoke(false);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            m_isStartPress = false;
            if (m_longPressTrigger)
            {
                m_longPressTrigger = false;
                m_onLongPress?.Invoke(false);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            m_onPointerClick?.Invoke(eventData);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (!m_isNeedScale)
            {
                return;
            }
            switch (state)
            {
                case SelectionState.Normal:
                case SelectionState.Highlighted:
                case SelectionState.Selected:
                    transform.localScale = m_Forme;
                    break;
                case SelectionState.Pressed:
                    transform.localScale = m_To;
                    break;
                case SelectionState.Disabled:
                    break;
            }
        }
    }

    public class ButtonPressEvent : UnityEvent<bool>
    {
    }

    public class ButtonPointerClickEvent : UnityEvent<PointerEventData>
    {
    }
}
