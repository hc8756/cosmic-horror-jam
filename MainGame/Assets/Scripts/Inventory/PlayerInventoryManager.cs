using System;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    public class PlayerInventoryManager : MonoBehaviour
    {
        public PlayerInventoryData PlayerInventoryData;
        public List<ItemController> StartingItems = new List<ItemController>();
        
        [Tooltip("Layer to set FPS weapon gameObjects to")]
        public LayerMask FpsWeaponLayer;

        [Tooltip("Delay before switching item a second time, to avoid recieving multiple inputs from mouse wheel")]
        public float WeaponSwitchDelay = 1f;

        PlayerInputHandler _inputHandler;
        PlayerCharacterController _playerCharacterController;
        InventorySystem[] _subsystems;
        float _timeStartedItemSwitch;
        int _newItemToSwitchTo;
        
        void Start()
        {
            _subsystems = GetComponents<InventorySystem>();
            if(_subsystems == null) throw new InvalidOperationException("You need at least one InventorySystem to use PlayerInventoryManager.");

            foreach(InventorySystem system in _subsystems)
            {
                system.Initialize(PlayerInventoryData);
            }


            PlayerInventoryData.ActiveItemIndex = -1;
            PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.Down;

            _inputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerWeaponsManager>(_inputHandler, this, gameObject);

            _playerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerWeaponsManager>(_playerCharacterController, this, gameObject);

            foreach (var item in StartingItems)
            {
                AddItem(item);
            }
            
            // Switch weapon in ascending order, so switch to first weapon
            SwitchItem(true);
        }
        
        void Update()
        {
            ItemSwitchState switchState = PlayerInventoryData.CurrentItemSwitchState;

            ItemController currentlyEquippedItem = PlayerInventoryData.GetActiveItem();
            if(currentlyEquippedItem == null) return;

            bool canSwitchItem = true;
            
            foreach(InventorySystem subsystem in _subsystems)
            {
                if(subsystem.SupportsItemType(currentlyEquippedItem))
                {
                    subsystem.OnActiveItemUpdate(currentlyEquippedItem);
                }
                
                if(!subsystem.CanSwitchItem(currentlyEquippedItem)) canSwitchItem = false;
            }

            // weapon switch handling - we can switch as long as a few things are true
            if (canSwitchItem)
            {
                int itemSwitchDirection = _inputHandler.GetItemSwitchDirection();
                
                if (itemSwitchDirection != 0)
                {
                    bool switchUp = itemSwitchDirection > 0;
                    SwitchItem(switchUp);

                    return;
                }

                // itemswitchrection was zero so go off of keys now
                itemSwitchDirection = _inputHandler.GetSelectWeaponInput();

                if (itemSwitchDirection == 0) return;

                if (PlayerInventoryData.GetItemAtSlotIndex(itemSwitchDirection - 1) != null)
                    SwitchToItemIndex(itemSwitchDirection - 1);

            }
        }

        public void LowerItem()
        {
            SwitchToItemIndex(-1);
        }

        // Iterate on all weapon slots to find the next valid weapon to switch to
        public void SwitchItem(bool ascendingOrder)
        {
            int newItemIndex = -1;
            int closestSlotDistance = PlayerInventoryData.Inventory.Length;

            for (int i = 0; i < PlayerInventoryData.Inventory.Length; i++)
            {
                // If the weapon at this slot is valid, calculate its "distance" from the active slot index (either in ascending or descending order)
                // and select it if it's the closest distance yet
                if (i != PlayerInventoryData.ActiveItemIndex && PlayerInventoryData.GetItemAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenItemSlots(PlayerInventoryData.ActiveItemIndex, i, ascendingOrder);

                    if (distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newItemIndex = i;
                    }
                }
            }

            // Handle switching to the new weapon index
            SwitchToItemIndex(newItemIndex);
        }

        private void LateUpdate()
        {
            UpdateItemSwitching();
        }

        // Updates the animated transition of switching items
        void UpdateItemSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            float switchingTimeFactor = 0f;

            if (WeaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - _timeStartedItemSwitch) / WeaponSwitchDelay);
            }

            // Handle transiting to new switch state
            if (switchingTimeFactor >= 1f)
            {
                if (PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.PutDownPrevious)
                {
                    // Deactivate old weapon
                    ItemController oldWeapon = PlayerInventoryData.GetItemAtSlotIndex(PlayerInventoryData.ActiveItemIndex);
                    if (oldWeapon != null)
                    {
                        oldWeapon.Equip(false);
                    }

                    PlayerInventoryData.ActiveItemIndex = _newItemToSwitchTo;
                    switchingTimeFactor = 0f;

                    // Activate new weapon
                    ItemController newWeapon = PlayerInventoryData.GetItemAtSlotIndex(PlayerInventoryData.ActiveItemIndex);
                    
                    foreach(InventorySystem subsystem in _subsystems)
                    {
                        if(subsystem.SupportsItemType(newWeapon))
                            subsystem.SwitchToItem(newWeapon);
                    }

                    if (newWeapon)
                    {
                        _timeStartedItemSwitch = Time.time;
                        PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.PutUpNew;
                    }
                    else
                    {
                        // if new weapon is null, don't follow through with putting weapon back up
                        PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.Down;
                    }
                }
                else if (PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.PutUpNew)
                {
                    PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.Up;
                }
            }

            foreach(InventorySystem system in _subsystems)
            {
                system.HandleItemTransitionMovements(PlayerInventoryData.CurrentItemSwitchState, switchingTimeFactor);
            }
        }

        // Switches to the given item index in item slots if the new index is a valid item that is different from our current one
        // Could be changed to switch no matter what, allowing duplicate items
        public void SwitchToItemIndex(int newWeaponIndex, bool force = false)
        {
            if (!(force || (newWeaponIndex != PlayerInventoryData.ActiveItemIndex && newWeaponIndex >= 0))) return;

            // Store data related to weapon switching animation
            _newItemToSwitchTo = newWeaponIndex;
            _timeStartedItemSwitch = Time.time;

            // TODO track acitve susbsytem
            foreach(InventorySystem subsystem in _subsystems)
            {
                ItemController currentlyEquipped = PlayerInventoryData.GetActiveItem();

                // Handle case of switching to a valid weapon for the first time (simply put it up without putting anything down first)
                if (currentlyEquipped == null)
                {
                    PlayerInventoryData.WeaponMainLocalPosition = PlayerInventoryData.DownWeaponPosition.localPosition;
                    PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.PutUpNew;
                    PlayerInventoryData.ActiveItemIndex = _newItemToSwitchTo;

                    ItemController newWeapon = PlayerInventoryData.GetItemAtSlotIndex(_newItemToSwitchTo);
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    PlayerInventoryData.CurrentItemSwitchState = ItemSwitchState.PutDownPrevious;
                }
            }
        }

        // Adds a weapon to our inventory
        public bool AddItem(ItemController itemControllerPrefab)
        {
            // search our weapon slots for the first free one, assign the weapon to it, and return true if we found one. Return false otherwise
            for (int i = 0; i < PlayerInventoryData.Inventory.Length; i++)
            {
                // only add the weapon if the slot is free
                if (PlayerInventoryData.Inventory[i] == null)
                {
                    // spawn the weapon prefab as child of the weapon socket
                    ItemController weaponInstance = Instantiate(itemControllerPrefab, PlayerInventoryData.WeaponParentSocket);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
                    weaponInstance.Owner = gameObject;
                    weaponInstance.SourcePrefab = itemControllerPrefab.gameObject;
                    weaponInstance.Equip(false);

                    // Assign the first person layer to the weapon
                    int layerIndex = Mathf.RoundToInt(Mathf.Log(FpsWeaponLayer.value, 2)); // This function converts a layermask to a layer index
                    foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t.gameObject.layer = layerIndex;
                    }

                    PlayerInventoryData.Inventory[i] = weaponInstance;

                    foreach(InventorySystem system in _subsystems)
                    {
                        system.OnAddItem(weaponInstance, i);
                    }

                    return true;
                }
            }

            // Handle auto-switching to weapon if no weapons currently
            if (PlayerInventoryData.GetActiveItem() == null)
            {
                SwitchItem(true);
            }

            return false;
        }

        public bool RemoveWeapon(ItemController itemInstance)
        {
            // Look through our slots for that weapon
            for (int i = 0; i < PlayerInventoryData.Inventory.Length; i++)
            {
                if (!(PlayerInventoryData.Inventory[i] == itemInstance)) continue;

                PlayerInventoryData.Inventory[i] = null;
                
                // Notify systems
                foreach(InventorySystem subsystem in _subsystems)
                {
                    subsystem.OnRemoveItem(itemInstance, i);
                }

                Destroy(itemInstance.gameObject);

                // Handle case of removing active weapon (switch to next weapon)
                if (i == PlayerInventoryData.ActiveItemIndex)
                {
                    SwitchItem(true);
                }

                return true;
            }

            return false;
        }

        // Calculates the "distance" between two weapon slot indexes
        // For example: if we had 5 weapon slots, the distance between slots #2 and #4 would be 2 in ascending order, and 3 in descending order
        int GetDistanceBetweenItemSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if (ascendingOrder)
            {
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            }
            else
            {
                distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
            }

            if (distanceBetweenSlots < 0)
            {
                distanceBetweenSlots = PlayerInventoryData.Inventory.Length + distanceBetweenSlots;
            }

            return distanceBetweenSlots;
        }

        void OnItemSwitched(ItemController newItem)
        {
            if (newItem != null)
            {
                newItem.Equip(true);
            }
        }
    }

    // Could be worth making a ScriptableObject if lots of systems end up wanting to use this
    [System.Serializable]
    public class PlayerInventoryData
    {
        public ItemController[] Inventory;
        
        [Header("References")] [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
        public Camera WeaponCamera;
        [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform WeaponParentSocket;

        [Tooltip("Position for weapons when active but not actively aiming")]
        public Transform DefaultWeaponPosition;

        [Tooltip("Position for innactive weapons")]
        public Transform DownWeaponPosition;

        public int ActiveItemIndex;
        public ItemSwitchState CurrentItemSwitchState;
        public Vector3 LastCharacterPosition;
        public Vector3 WeaponMainLocalPosition;
        public Vector3 WeaponBobLocalPosition;

        public ItemController GetActiveItem() => GetItemAtSlotIndex(ActiveItemIndex);

        public ItemController GetItemAtSlotIndex(int index)
        {
            if (index >= 0 && index < Inventory.Length)
            {
                return Inventory[index];
            }

            return null;
        }

    }

    public enum ItemSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }

}