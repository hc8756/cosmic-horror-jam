using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

public abstract class InventorySystem : MonoBehaviour
{
    public PlayerInventoryData PlayerInventoryData;

    public abstract bool SupportsItemType(ItemController itemController);

    public virtual bool CanSwitchItem(ItemController currentlyEquippedItem)
    {
        return true;
    }

    public virtual void OnActiveItemUpdate(ItemController itemController)
    {
        
    }

    public virtual void HandleItemTransitionMovements(ItemSwitchState switchState, float switchingTimeFactor)
    {
        
    }

    public virtual void Initialize(PlayerInventoryData playerInventoryData)
    {
        this.PlayerInventoryData = playerInventoryData;
    }

    public virtual void OnAddItem(ItemController itemController, int index)
    {

    }

    public virtual void OnRemoveItem(ItemController itemController, int index)
    {

    }

    public virtual void SwitchToItem(ItemController newItem)
    {

    }
}
