using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEventListener :  EventHandler<CustomObjectData>
{
    public override void RaiseEvent(CustomObjectData data)
    {
        Debug.Log($"Object touched name is {data.objectTouched.name} player name is {data.playerWhoTouchedObject.name} message is {data.message}");
    }
}
