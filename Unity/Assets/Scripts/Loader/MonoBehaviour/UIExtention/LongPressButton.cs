using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[AddComponentMenu("UI/LongPressButton")]
public class LongPressButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    [FormerlySerializedAs("onClick")]
    //[SerializeField]
    private LongPressButton.LongPressButtonClickedEvent m_OnClick = new LongPressButton.LongPressButtonClickedEvent();
    private LongPressButton.LongPressButtonLongPressedEvent m_OnLongPress = new LongPressButton.LongPressButtonLongPressedEvent();
    public float durationThreshold = 1.0f;
    public bool isLooped = true;
    public float interval = 0.2f;
    private bool restart = true;
    private int curTimes = 0;

    //public UnityEvent onLongPress = new UnityEvent();
    //public UnityEvent onClick = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    protected LongPressButton()
    {
    }

    /// <summary>
    ///   <para>UnityEvent that is triggered when the button is pressed.</para>
    /// </summary>
    public LongPressButton.LongPressButtonClickedEvent onClick
    {
        get
        {
            return this.m_OnClick;
        }
        set
        {
            this.m_OnClick = value;
        }
    }

    /// <summary>
    ///   <para>UnityEvent that is triggered when the button is pressed.</para>
    /// </summary>
    public LongPressButton.LongPressButtonLongPressedEvent onLongPress
    {
        get
        {
            return this.m_OnLongPress;
        }
        set
        {
            this.m_OnLongPress = value;
        }
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
                //
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

    /// <summary>
    ///   <para>Function definition for a button click event.</para>
    /// </summary>
    [Serializable]
    public class LongPressButtonClickedEvent : UnityEvent
    {
    }

    /// <summary>
    ///   <para>Function definition for a button long press event.</para>
    /// </summary>
    [Serializable]
    public class LongPressButtonLongPressedEvent : UnityEvent
    {
    }
}

