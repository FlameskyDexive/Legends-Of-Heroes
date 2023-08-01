using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace ET
{
    public class EUIButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        #region ö��

        #region �����Ч
        public enum AudioClcikEnum
        {
            None = 0,       //����Ч
            Comfirm = 1,    //ȷ��
            Cancel = 2,     //ȡ��
            GoHome = 3,     //ǰ����ҳ
            GoExplore = 4,  //ǰ�����ͼ
        }
        #endregion �����Ч

        #endregion ö��

        #region �ֶ�
        private bool m_isPress = false;

        public AudioClcikEnum m_audioClick = AudioClcikEnum.None;

        //�Ƿ���Ҫ������
        [Tooltip("�Ƿ���Ҫ������")] public bool m_isNeedInterval = false;
        public float m_IntervalTime = 0;
        public bool m_isClick = false;

        //�Ƿ���Ҫ����
        [Tooltip("�Ƿ���Ҫ����")] public bool m_isNeedScale = false;
        private Vector3 m_Forme = Vector3.one;
        public Vector3 m_To = new Vector3(0.8f, 0.8f, 0.8f);

        //public int a = 0;

        #region �������
        //�����¼�
        public ButtonPressEvent m_onLongPress { get; set; }
        private bool m_isStartPress = false;
        private float m_curPointDownTime = 0.0f;
        private bool m_longPressTrigger = false;
        [Tooltip("���������¼�ʱ����")] public float m_longPressTime = 0.5f;
        #endregion �������

        public ButtonPointerClickEvent m_onPointerClick { get; set; }

        #endregion �ֶ�

        #region ����
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

        #endregion ����

        #region ��������
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

        #endregion ��������

        #region ���ⷽ��
        // ���ð�ѹ״̬��ͼƬ��ʾ
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
        #endregion ���ⷽ��

        #region ���ڷ���
        // ���ð�ѹ״̬��ͼƬ��ʾ
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

        #region �������
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
            //����ˢ�µ�ǰʱ��
            base.OnPointerDown(eventData);
            m_curPointDownTime = Time.time;
            m_isStartPress = true;
            m_longPressTrigger = false;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //��ָ̧�𣬽�����ʼ����
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
            //��ָ�Ƴ���������ʼ����
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
        #endregion �������

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
        #endregion ���ڷ���
    }

    public class ButtonPressEvent : UnityEvent<bool>
    {
    }

    public class ButtonPointerClickEvent : UnityEvent<PointerEventData>
    {
    }
}
