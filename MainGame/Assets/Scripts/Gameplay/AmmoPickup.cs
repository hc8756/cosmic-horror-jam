using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class AmmoPickup : Pickup
    {
        public WeaponController Weapon;
        public int BulletCount = 30;

        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PlayerWeaponsManager playerWeaponsManager = byPlayer.GetComponent<PlayerWeaponsManager>();

            if (!playerWeaponsManager) return;

            WeaponController weapon = playerWeaponsManager.PlayerHasWeapon(Weapon);
            if (weapon != null)
            {
                weapon.AddCarriablePhysicalBullets(BulletCount);

                // TODO pass proper stuff in
                pickupEvent.Raise(this.gameObject);

                PlayPickupFeedback();
                Destroy(gameObject);
            }
        }
    }
}
