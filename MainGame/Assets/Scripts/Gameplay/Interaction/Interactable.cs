using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

public abstract class Interactable : MonoBehaviour
{
    public CrosshairData crosshairData;
    

    public virtual void OnPlayerHover(Ray ray, RaycastHit hit)
    {
    }
}
