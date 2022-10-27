using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

public class HiddenItemPickup : Interactable
{
    public string itemName;
        
    public override void OnPlayerHover(Ray ray, RaycastHit hit)
    {
        if(!Input.GetMouseButton(0)) return;

        PlayerInventoryManager playerInventoryManager = GameObject.FindObjectOfType<PlayerInventoryManager>();
        if(!playerInventoryManager) return;

        playerInventoryManager.AddHiddenItem(itemName);

        Destroy(gameObject);
    }
}
