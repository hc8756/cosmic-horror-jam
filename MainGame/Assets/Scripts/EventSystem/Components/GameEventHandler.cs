using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameEventHandler : MonoBehaviour, IGameEventListener
{
    public GameEvent gameEvent;
    public UnityEvent unityEvent;
    public UltEvents.UltEvent ultEvent;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    public void OnEventRaised()
    {
        unityEvent.Invoke();

        if(ultEvent != null)
            ultEvent.Invoke();
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }
}
