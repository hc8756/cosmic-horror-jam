using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class NotificationHUDManager : EventHandler<string>
    {
        public RectTransform NotificationPanel;
        public GameObject NotificationPrefab;

        void Awake()
        {
            PlayerInventoryManager playerInventoryManager = UnityHelper.FindObjectOfTypeOrThrow<PlayerInventoryManager>();
            playerInventoryManager.OnItemAdded += OnPickupItem;

            PlayerWeaponsManager playerWeaponsManager = UnityHelper.FindObjectOfTypeOrThrow<PlayerWeaponsManager>();
            playerWeaponsManager.OnAddedWeapon += OnPickupWeapon;

        }
        
        public override void RaiseEvent(string notification)
        {
            HandleNewNotification(notification);
        }

        private void HandleNewNotification(string notification)
        {
            if (!string.IsNullOrEmpty(notification))
                CreateNotification(notification);
        }

        void OnPickupWeapon(WeaponController weaponController, int index)
        {
            if (index != 0)
                CreateNotification("Picked up weapon : " + weaponController.WeaponName);
        }
        
        void OnPickupItem(string name)
        {
            CreateNotification("Picked up " + name);
        }

        public void CreateNotification(string text)
        {
            GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
            notificationInstance.transform.SetSiblingIndex(0);

            NotificationToast toast = notificationInstance.GetComponent<NotificationToast>();
            if (toast)
            {
                toast.Initialize(text);
            }
        }
    }
}