using UnityEngine;using System.Collections;using UnityEngine.EventSystems;using System.Collections.Generic;








/// <summary>/// UGUI事件监听类/// </summary>public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{    public delegate void VoidDelegate(GameObject go);    public delegate void VectorDelegate(GameObject go, Vector2 delta);    public VoidDelegate onClick;    public VoidDelegate onDown;    public VoidDelegate onEnter;    public VoidDelegate onExit;    public VoidDelegate onUp;    public VoidDelegate onSelect;    public VoidDelegate onUpdateSelect;

    public VectorDelegate onDrag;    public VoidDelegate onDragOut;


    static public EventTriggerListener Get(GameObject go)    {        if (go == null)
        {            Debug.LogError("EventTriggerListener_go_is_NULL");            return null;        }        else
        {            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();            if (listener == null) listener = go.AddComponent<EventTriggerListener>();            return listener;        }    }

    public override void OnDrag(PointerEventData eventData)    {        if (onDrag != null) onDrag(gameObject, eventData.delta);    }

    public override void OnEndDrag(PointerEventData eventData)    {        if (onDragOut != null) onDragOut(gameObject);    }

    public override void OnPointerClick(PointerEventData eventData)    {        if (onClick != null) onClick(gameObject);    }    public override void OnPointerDown(PointerEventData eventData)
    {        if (onDown != null) onDown(gameObject);    }    public override void OnPointerEnter(PointerEventData eventData)
    {        if (onEnter != null) onEnter(gameObject);    }    public override void OnPointerExit(PointerEventData eventData)
    {        if (onExit != null) onExit(gameObject);    }    public override void OnPointerUp(PointerEventData eventData)
    {        if (onUp != null) onUp(gameObject);    }    public override void OnSelect(BaseEventData eventData)
    {        if (onSelect != null) onSelect(gameObject);    }    public override void OnUpdateSelected(BaseEventData eventData)
    {        if (onUpdateSelect != null) onUpdateSelect(gameObject);    }}