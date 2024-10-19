using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace ET
{
    public class EUIButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        #region 枚举

        #region 点击音效
        public enum AudioClcikEnum
        {
            None = 0,       //无音效
            Comfirm = 1,    //确定
            Cancel = 2,     //取消
            GoHome = 3,     //前往首页
            GoExplore = 4,  //前往大地图
        }
        #endregion 点击音效

        #endregion 枚举

        #region 字段
        private bool m_isPress = false;

        public AudioClcikEnum m_audioClick = AudioClcikEnum.None;

        //是否需要点击间隔
        [Tooltip("是否需要点击间隔")] public bool m_isNeedInterval = false;
        public float m_IntervalTime = 0;
        public bool m_isClick = false;

        //是否需要缩放
        [Tooltip("是否需要缩放")] public bool m_isNeedScale = false;
        private Vector3 m_Forme = Vector3.one;
        public Vector3 m_To = new Vector3(0.8f, 0.8f, 0.8f);

        //public int a = 0;

        #region 长按相关
        //长按事件
        public ButtonPressEvent m_onLongPress { get; set; }
        private bool m_isStartPress = false;
        private float m_curPointDownTime = 0.0f;
        private bool m_longPressTrigger = false;
        [Tooltip("长按触发事件时间间隔")] public float m_longPressTime = 0.5f;
        #endregion 长按相关

        public ButtonPointerClickEvent m_onPointerClick { get; set; }

        #endregion 字段

        #region 属性
        public bool IsPress { get { return m_isPress; } private set { m_isPress = value; } }

        public ButtonPressEvent OnLongPress
        {
            get
            {
                if (m_onLongPress == null)
                {
                    m_onLongPress = new ButtonPressEvent();
                }
                return m_onLongPress;
            }
            set
            {
                m_onLongPress = value;
            }
        }

        public ButtonPointerClickEvent OnPointClick
        {
            get
            {
                if (m_onPointerClick == null)
                {
                    m_onPointerClick = new ButtonPointerClickEvent();
                }
                return m_onPointerClick;
            }
            set
            {
                m_onPointerClick = value;
            }
        }

        #endregion 属性

        #region 生命周期
        protected override void Awake()
        {
            base.Awake();

            if (m_onLongPress == null)
            {
                m_onLongPress = new ButtonPressEvent();
            }
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
                if (m_IntervalTime > 0.5)
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

        #endregion 生命周期

        #region 对外方法
        // 设置按压状态的图片显示
        public void SetPress(bool isPress)
        {
            //Logger.LogErrorFormat("ButtonCus.SetPress : isPress({0}) :  ", isPress);
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
        #endregion 对外方法

        #region 对内方法
        // 设置按压状态的图片显示
        public void ClickCallBack()
        {
            if (m_isNeedInterval)
            {
                m_isClick = true;
                this.interactable = false;
            }

            //if (m_audioClick == AudioClcikEnum.None)
            //{
            //    return;
            //}
            //if (m_audioClick == AudioClcikEnum.Comfirm)
            //{
            //    AudioManager.Instance.Play(AudioDefine.RNSound_Confirm_0001_Aud_2DS, false, null);
            //}
            //else if (m_audioClick == AudioClcikEnum.Cancel)
            //{
            //    AudioManager.Instance.Play(AudioDefine.RNSound_Cancel_0001_Aud_2DS, false, null);
            //}
            //else if (m_audioClick == AudioClcikEnum.GoHome)
            //{
            //    AudioManager.Instance.Play(AudioDefine.RNSound_Home_0001_Aud_2DS, false, null);
            //}
            //else if (m_audioClick == AudioClcikEnum.GoExplore)
            //{
            //    AudioManager.Instance.Play(AudioDefine.RNSound_Galaxy_0001_Aud_2DS, false, null);
            //}
        }

        #region 长按相关
        private void CheckIsLongPress()
        {
            if (m_isStartPress && !m_longPressTrigger)
            {
                if (Time.time > m_curPointDownTime + m_longPressTime)
                {
                    m_longPressTrigger = true;
                    m_isStartPress = false;
                    if (m_onLongPress != null)
                    {
                        m_onLongPress.Invoke(true);
                    }
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //按下刷新当前时间
            base.OnPointerDown(eventData);
            m_curPointDownTime = Time.time;
            m_isStartPress = true;
            m_longPressTrigger = false;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //手指抬起，结束开始长按
            base.OnPointerUp(eventData);
            m_isStartPress = false;
            if (m_longPressTrigger)
            {
                m_longPressTrigger = false;
                if (m_onLongPress != null)
                {
                    m_onLongPress.Invoke(false);
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //手指移除，结束开始长按
            base.OnPointerExit(eventData);
            m_isStartPress = false;
            if (m_longPressTrigger)
            {
                m_longPressTrigger = false;
                if (m_onLongPress != null)
                {
                    m_onLongPress.Invoke(false);
                }
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            m_onPointerClick?.Invoke(eventData);
        }
        #endregion 长按相关

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (!m_isNeedScale)
                return;
            switch (state)
            {
                case SelectionState.Normal:
                    transform.localScale = m_Forme;
                    break;
                case SelectionState.Highlighted:
                    transform.localScale = m_Forme;
                    break;
                case SelectionState.Pressed:
                    transform.localScale = m_To;
                    break;
                case SelectionState.Selected:
                    transform.localScale = m_Forme;
                    break;
                case SelectionState.Disabled:
                    break;
                default:
                    break;
            }
        }
        #endregion 对内方法
    }

    public class ButtonPressEvent : UnityEvent<bool>
    {
    }

    public class ButtonPointerClickEvent : UnityEvent<PointerEventData>
    {
    }
}
