using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomObjectData
{
    public GameObject objectTouched;
    public GameObject playerWhoTouchedObject;
    public string message;
}

[CreateAssetMenu( menuName = "Events/Custom/MyCustomEvent")]
public class MyCustomEvent : Event<CustomObjectData> { }
