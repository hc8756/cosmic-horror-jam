using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameEventHandler : MonoBehaviour, IGameEventListener
{
    public GameEvent gameEvent;
    public UnityEvent unityEvent;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    public void OnEventRaised()
    {
        unityEvent.Invoke();
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }
}
