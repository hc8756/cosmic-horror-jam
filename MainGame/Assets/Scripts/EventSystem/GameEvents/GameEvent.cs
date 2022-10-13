using System;
using System.Collections.Generic;
using UnityEngine;

// Game event that passes no data
[CreateAssetMenu( menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    // NOTE: maybe be worth converting to hashset if performance suffers
    private readonly List<IGameEventListener> _eventListeners = new List<IGameEventListener>();

    public void Raise()
    {
        for(int i = _eventListeners.Count -1; i >= 0; i--)
            _eventListeners[i].OnEventRaised();
    }

    public void RegisterListener(IGameEventListener listener)
    {
        _eventListeners.Add(listener); 
    }

    public void UnregisterListener(IGameEventListener listener)
    { 
        _eventListeners.Remove(listener); 
    }
}
