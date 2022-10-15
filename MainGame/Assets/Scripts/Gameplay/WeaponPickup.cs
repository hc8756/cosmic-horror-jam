using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class WeaponPickup : Pickup
    {
        [Tooltip("The prefab for the weapon that will be added to the player on pickup")]
        public WeaponController WeaponPrefab;

        protected override void Start()
        {
            base.Start();

            // Set all children layers to default (to prefent seeing weapons through meshes)
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                    t.gameObject.layer = 0;
            }
        }

        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PlayerInventoryManager playerInventoryManager = byPlayer.GetComponent<PlayerInventoryManager>();
            if(!playerInventoryManager) return;

            playerInventoryManager.AddItem(WeaponPrefab);

            PlayPickupFeedback();
            Destroy(gameObject);

        }
    }
}