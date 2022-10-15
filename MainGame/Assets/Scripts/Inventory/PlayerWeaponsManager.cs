using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(PlayerInputHandler), typeof(PlayerInventoryManager))]
    public class PlayerWeaponsManager : InventorySystem
    {
        [Tooltip("Position for weapons when aiming")]
        public Transform AimingWeaponPosition;

        [Tooltip("Distance the weapon bobs when aiming")]
        public float AimingBobMultiplier = 0.02f;

        [Header("Weapon Recoil")]
        [Tooltip("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        public float RecoilSharpness = 50f;

        [Tooltip("Maximum distance the recoil can affect the weapon")]
        public float MaxRecoilDistance = 0.5f;

        [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
        public float RecoilRestitutionSharpness = 10f;

        [Header("Misc")] [Tooltip("Speed at which the aiming animatoin is played")]
        public float AimingAnimationSpeed = 10f;

        [Tooltip("Field of view when not aiming")]
        public float DefaultFov = 60f;


        // Events
        public UnityAction<WeaponController> OnSwitchedToWeapon;
        public UnityAction<WeaponController, int> OnAddedWeapon;
        public UnityAction<WeaponController, int> OnRemovedWeapon;

        public bool IsAiming { get; private set; }
        public bool IsPointingAtEnemy { get; private set; }

        PlayerInputHandler _inputHandler;
        PlayerCharacterController _playerCharacterController;
        Vector3 _weaponRecoilLocalPosition;
        Vector3 _accumulatedRecoil;

        public override void Initialize(PlayerInventoryData playerInventoryData)
        {
            base.Initialize(playerInventoryData);

            _inputHandler = gameObject.GetComponentOrThrow<PlayerInputHandler>();
            _playerCharacterController = gameObject.GetComponentOrThrow<PlayerCharacterController>();
            _playerCharacterController.SetFov(DefaultFov);
        }
        public override void OnAddItem(ItemController item, int index)
        {
            Debug.Log($"Addeded weapon {item.WeaponName}");
            OnAddedWeapon.Invoke(item as WeaponController, index);
        }

        public override bool SupportsItemType(ItemController itemController)
        {
            return itemController is WeaponController;
        }

        public WeaponController GetActiveWeapon()
        {
            if(PlayerInventoryData.GetActiveItem() is WeaponController weaponController) return weaponController;
            return null;
        }

        public WeaponController PlayerHasWeapon(WeaponController weaponPrefab)
        {
            // TODO a beter method on PlayerInventoryData that uses types and stuff
            for (var index = 0; index < PlayerInventoryData.Inventory.Length; index++)
            {
                var w = PlayerInventoryData.Inventory[index];
                if((w is WeaponController controller) && controller.SourcePrefab == weaponPrefab.gameObject) return controller;
            }

            return null;
        }

        public override void SwitchToItem(ItemController newItem)
        {
            OnSwitchedToWeapon.Invoke(newItem as WeaponController);
            newItem.Equip(true);
        }

        public override void OnActiveItemUpdate(ItemController itemController)
        {
            PlayerInventoryData.BobMultiplier = IsAiming ? AimingBobMultiplier : 1f;

            WeaponController activeWeapon = (WeaponController)itemController;

            if (activeWeapon != null && activeWeapon.IsReloading)
                return;

            if (activeWeapon != null && PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.Up)
            {
                if (!activeWeapon.AutomaticReload && _inputHandler.GetReloadButtonDown() && activeWeapon.CurrentAmmoRatio < 1.0f)
                {
                    IsAiming = false;
                    activeWeapon.StartReloadAnimation();
                    return;
                }
                // handle aiming down sights
                IsAiming = _inputHandler.GetAimInputHeld();

                // handle shooting
                bool hasFired = activeWeapon.HandleShootInputs(
                    _inputHandler.GetFireInputDown(),
                    _inputHandler.GetFireInputHeld(),
                    _inputHandler.GetFireInputReleased());

                // Handle accumulating recoil
                if (hasFired)
                {
                    _accumulatedRecoil += Vector3.back * activeWeapon.RecoilForce;
                    _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, MaxRecoilDistance);
                }
            }

            // Pointing at enemy handling
            IsPointingAtEnemy = false;
            if (activeWeapon)
            {
                if (Physics.Raycast(PlayerInventoryData.WeaponCamera.transform.position, PlayerInventoryData.WeaponCamera.transform.forward, out RaycastHit hit,
                    1000, -1, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponentInParent<Health>() != null)
                    {
                        IsPointingAtEnemy = true;
                    }
                }
            }
        }

        // Update various animated features in LateUpdate because it needs to override the animated arm position
        void LateUpdate()
        {
            UpdateWeaponAiming();
            UpdateWeaponRecoil();

            // Set final weapon socket position based on all the combined animation influences
            PlayerInventoryData.WeaponParentSocket.localPosition =
                PlayerInventoryData.WeaponMainLocalPosition + PlayerInventoryData.WeaponBobLocalPosition + _weaponRecoilLocalPosition;
        }

        public override void HandleItemTransitionMovements(ItemSwitchState switchState, float switchingTimeFactor)
        {
            // Handle moving the weapon socket position for the animated weapon switching
            if (PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.PutDownPrevious)
            {
                PlayerInventoryData.WeaponMainLocalPosition = Vector3.Lerp(PlayerInventoryData.DefaultWeaponPosition.localPosition,
                    PlayerInventoryData.DownWeaponPosition.localPosition, switchingTimeFactor);
            }
            else if (PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.PutUpNew)
            {
                PlayerInventoryData.WeaponMainLocalPosition = Vector3.Lerp(PlayerInventoryData.DownWeaponPosition.localPosition,
                    PlayerInventoryData.DefaultWeaponPosition.localPosition, switchingTimeFactor);
            }
        }

        // Example of custom item switching logic
        public override bool CanSwitchItem(ItemController currentlyEquippedItem)
        {
            if(!(currentlyEquippedItem is WeaponController)) return base.CanSwitchItem(currentlyEquippedItem);

            WeaponController weaponController = currentlyEquippedItem as WeaponController;

            return base.CanSwitchItem(currentlyEquippedItem) && !IsAiming && !weaponController.IsCharging;
        }

        // Updates weapon position and camera FoV for the aiming transition
        void UpdateWeaponAiming()
        {
            if (PlayerInventoryData.CurrentItemSwitchState == ItemSwitchState.Up)
            {
                WeaponController activeWeapon = GetActiveWeapon();
                if (IsAiming && activeWeapon)
                {
                    PlayerInventoryData.WeaponMainLocalPosition = Vector3.Lerp(PlayerInventoryData.WeaponMainLocalPosition,
                        AimingWeaponPosition.localPosition + activeWeapon.AimOffset,
                        AimingAnimationSpeed * Time.deltaTime);
                    _playerCharacterController.SetFov(Mathf.Lerp(_playerCharacterController.PlayerCamera.fieldOfView,
                        activeWeapon.AimZoomRatio * DefaultFov, AimingAnimationSpeed * Time.deltaTime));
                }
                else
                {
                    PlayerInventoryData.WeaponMainLocalPosition = Vector3.Lerp(PlayerInventoryData.WeaponMainLocalPosition,
                        PlayerInventoryData.DefaultWeaponPosition.localPosition, AimingAnimationSpeed * Time.deltaTime);
                    _playerCharacterController.SetFov(Mathf.Lerp(_playerCharacterController.PlayerCamera.fieldOfView, DefaultFov,
                        AimingAnimationSpeed * Time.deltaTime));
                }
            }
        }

        // Updates the weapon recoil animation
        void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (_weaponRecoilLocalPosition.z >= _accumulatedRecoil.z * 0.99f)
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, _accumulatedRecoil,
                    RecoilSharpness * Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, Vector3.zero,
                    RecoilRestitutionSharpness * Time.deltaTime);
                _accumulatedRecoil = _weaponRecoilLocalPosition;
            }
        }
    }
}