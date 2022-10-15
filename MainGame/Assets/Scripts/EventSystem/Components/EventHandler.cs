using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventHandler<T> : MonoBehaviour, IGameEventListener<T>
{
    // If you assing to this unity event, it will be inboked when the event is raised.
    public UnityEvent<T> unityEvent;

    public Event<T> gameEvent;

    public virtual void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    public void OnEventRaised(T data)
    {
        unityEvent.Invoke(data);

        RaiseEvent(data);
    }
    
    public abstract void RaiseEvent(T data);

    public virtual void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }
}
