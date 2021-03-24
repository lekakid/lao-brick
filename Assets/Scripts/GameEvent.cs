using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : ScriptableObject
{
    List<GameEventListener> listeners = new List<GameEventListener>();

    public void RegiesterListener(GameEventListener listener) {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener) {
        listeners.Remove(listener);
    }

    public void Invoke() {
        if(listeners.Count == 0) return;

        for(int i = 0; i < listeners.Count; i++) {
            listeners[i].OnEventRaised();
        }
    }
}
