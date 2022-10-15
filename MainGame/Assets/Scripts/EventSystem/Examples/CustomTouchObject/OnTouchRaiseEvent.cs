using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchRaiseEvent : MonoBehaviour
{
    public MyCustomEvent customEvent;

    void OnTriggerEnter(Collider other)
    {
        CustomObjectData message = new CustomObjectData()
        {
            playerWhoTouchedObject = other.gameObject,
            objectTouched = this.gameObject,
            message = "hello!"
        };

        customEvent.Raise(message);
    }
}
