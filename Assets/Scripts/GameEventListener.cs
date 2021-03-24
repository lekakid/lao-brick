using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent GameEvent;
    public UnityEvent Callbacks;

    private void OnEnable() {
        GameEvent.RegiesterListener(this);
    }

    private void OnDisable() {
        GameEvent.UnregisterListener(this);
    }

    public void OnEventRaised() {
        Callbacks.Invoke();
    }
}
