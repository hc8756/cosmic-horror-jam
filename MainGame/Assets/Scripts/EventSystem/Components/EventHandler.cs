using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler<T> : MonoBehaviour, IGameEventListener<T>
{
    public UnityEvent<T> unityEvent;

    public Event<T> gameEvent;

    public virtual void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    public virtual void OnEventRaised(T data)
    {
        unityEvent.Invoke(data);
    }

    public virtual void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }
}
