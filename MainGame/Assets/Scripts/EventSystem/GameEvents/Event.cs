using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event<T> : ScriptableObject
{
    private List<IGameEventListener<T>> _eventListeners = new List<IGameEventListener<T>>();

    public virtual void Raise(T data)
    {
        for(int i = _eventListeners.Count -1; i >= 0; i--)
            _eventListeners[i].OnEventRaised(data);
    }
    public virtual void RegisterListener(IGameEventListener<T> listener)
    {
        _eventListeners.Add(listener); 

    }
    public virtual void UnregisterListener(IGameEventListener<T> listener)
    {
        _eventListeners.Remove(listener); 
    }
}
