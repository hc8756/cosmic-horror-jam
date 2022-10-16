using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseEventOnClick : Interactable
{
    public GameEvent gameEvent;

    public override void OnPlayerHover(Ray ray, RaycastHit hit)
    {
        if(Input.GetMouseButtonDown(0))
        {
            gameEvent.Raise();
        }
    }
}
