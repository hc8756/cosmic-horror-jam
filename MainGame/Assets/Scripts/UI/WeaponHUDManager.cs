using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying weapon ammo")]
        public RectTransform AmmoPanel;

        [Tooltip("Prefab for displaying weapon ammo")]
        public GameObject AmmoCounterPrefab;

        PlayerWeaponsManager _playerWeaponsManager;
        List<AmmoCounter> _ammoCounters = new List<AmmoCounter>();

        void Start()
        {
            _playerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, WeaponHUDManager>(_playerWeaponsManager, this);
            
            int i = 0;
            foreach(ItemController item in _playerWeaponsManager.PlayerInventoryData.Inventory)
            {
                if(item is WeaponController w)
                {
                    AddWeapon(w, i);
                }
                i++;
            }

            WeaponController activeWeapon = _playerWeaponsManager.GetActiveWeapon();

            if (activeWeapon)
            {
                ChangeWeapon(activeWeapon);
            }

            _playerWeaponsManager.OnAddedWeapon += AddWeapon;
            _playerWeaponsManager.OnRemovedWeapon += RemoveWeapon;
            _playerWeaponsManager.OnSwitchedToWeapon += ChangeWeapon;
        }

        void AddWeapon(WeaponController newWeapon, int weaponIndex)
        {
            Debug.Log("Adding weapon to UI now!");
            GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
            AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();
            DebugUtility.HandleErrorIfNullGetComponent<AmmoCounter, WeaponHUDManager>(newAmmoCounter, this,
                ammoCounterInstance.gameObject);

            newAmmoCounter.Initialize(newWeapon, weaponIndex);

            _ammoCounters.Add(newAmmoCounter);
        }

        void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
        {
            int foundCounterIndex = -1;
            for (int i = 0; i < _ammoCounters.Count; i++)
            {
                if (_ammoCounters[i].WeaponCounterIndex == weaponIndex)
                {
                    foundCounterIndex = i;
                    Destroy(_ammoCounters[i].gameObject);
                }
            }

            if (foundCounterIndex >= 0)
            {
                _ammoCounters.RemoveAt(foundCounterIndex);
            }
        }

        void ChangeWeapon(WeaponController weapon)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AmmoPanel);
        }
    }
}