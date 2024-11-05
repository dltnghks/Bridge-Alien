using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action OnClickHandler = null;
    public Action OnPressedHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;
    public Action OnBeginDragHandler = null;
    public Action OnDragHandler = null;
    public Action OnEndDragHandler = null;
    public static Action CheckIfShapeCanBePlaced;
    
    private bool _pressed = false;

    private void Update()
    {
        if (_pressed)
        {
            OnPressedHandler?.Invoke();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        OnPointerDownHandler?.Invoke();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _pressed = false;
        OnBeginDragHandler?.Invoke();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _pressed = false;
        OnDragHandler?.Invoke();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        _pressed = false;
        OnEndDragHandler?.Invoke();
    }
}
