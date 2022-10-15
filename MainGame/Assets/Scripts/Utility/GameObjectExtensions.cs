using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityHelper
{
    // TODO TryGetComponent is probably faster
    public static T GetComponentOrThrow<T>(this GameObject gameObject)
    {
        T comp = gameObject.GetComponent<T>();
        if(comp == null) throw new InvalidOperationException($"Component {typeof(T).Name} doesn't exist on GameObject {gameObject.name}.");
        return comp;
    }

    public static T FindObjectOfTypeOrThrow<T>() where T : UnityEngine.Object
    {
        UnityEngine.Object o = UnityEngine.Object.FindObjectOfType<T>();
        if(o is T foundType) return foundType;
        throw new InvalidOperationException("Could not find object of type {typeof(T).FullName!");
    }
}
