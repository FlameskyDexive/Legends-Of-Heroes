using System;
using ETModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[AddComponentMenu("UI/IntervalButton")]
public class IntervalButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
    [Tooltip("How long must pointer be down on this object to trigger a interval press")]
    [FormerlySerializedAs("onClick")]
    //[SerializeField]
    private IntervalButton.IntervalButtonClickedEvent m_OnClick = new IntervalButton.IntervalButtonClickedEvent();

    //private IntervalButton intervalButton;
    public bool isGreyImage = false;
    public bool isInterval = false;
    public float interval = 1.8f;
    private float temInterval = 1.8f;
    private bool canInvoke = true;
    private bool isPointerDown = false;
    //private bool isClicked = false;

    protected IntervalButton()
    {
    }

    void Start()
    {
        //this.intervalButton = this;
    }

    /// <summary>
    ///   <para>UnityEvent that is triggered when the button is pressed.</para>
    /// </summary>
    public IntervalButton.IntervalButtonClickedEvent onClick
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
    

    private void Update()
    {
        if (!this.canInvoke)
        {
            this.temInterval -= Time.deltaTime;
            if (this.temInterval <= 0)
            {
                this.canInvoke = true;
                //this.isClicked = false;
                if(this.GetComponent<Image>()!= null)
                {
                    this.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    //Log.Error($"-00--not contains Image");
                }
                //this.temInterval = this.interval;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isInterval && this.canInvoke)
        {
            this.temInterval = this.interval;
            onClick.Invoke();
            //this.isClicked = true;
            if (this.isGreyImage)
            {
                if (this.GetComponent<Image>() != null)
                {
                    this.GetComponent<Image>().color = Color.grey;
                }
                else
                {
                    //Log.Error($"-11--not contains Image");
                }
            }
            this.canInvoke = false;
        }
    }

    /// <summary>
    ///   <para>Function definition for a button click event.</para>
    /// </summary>
    [Serializable]
    public class IntervalButtonClickedEvent : UnityEvent
    {
    }
    
}

