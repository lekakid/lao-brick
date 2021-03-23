using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DPadButton : Selectable
{
    [System.Serializable]
    public class DirectionEvent : UnityEvent<Vector2> {}

    [SerializeField]
    Vector2 direction;

    public DirectionEvent OnButtonDown;
    public UnityEvent OnButtonUp;

    public override void OnPointerDown(PointerEventData eventData) {
        if(!IsActive() || !IsInteractable())
            return;

        base.OnPointerDown(eventData);
        OnButtonDown.Invoke(direction);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
        OnButtonUp.Invoke();
    }
}
