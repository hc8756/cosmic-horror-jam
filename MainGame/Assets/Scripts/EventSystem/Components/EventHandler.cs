using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventHandler<T> : MonoBehaviour, IGameEventListener<T>
{
    // If you assing to this unity event, it will be inboked when the event is raised.
    // You can use either the ultEvent or unityEvent
    public UnityEvent<T> unityEvent;
    public UltEvents.UltEvent<T> ultEvent;

    public Event<T> gameEvent;

    public virtual void OnEnable()
    {
        if(gameEvent)
            gameEvent.RegisterListener(this);
    }

    public void OnEventRaised(T data)
    {
        unityEvent.Invoke(data);

        if(ultEvent != null)
            ultEvent.Invoke(data);

        RaiseEvent(data);
    }
    
    public abstract void RaiseEvent(T data);

    public virtual void OnDisable()
    {
        if(gameEvent)
            gameEvent.UnregisterListener(this);
    }
}
