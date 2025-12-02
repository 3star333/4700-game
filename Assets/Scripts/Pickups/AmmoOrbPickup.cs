using UnityEngine;
using GothicShooter.Core;

namespace GothicShooter.Pickups
{
    /// <summary>
    /// Simple ammo pickup: adds reserve ammo to the first Weapon found on the player.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AmmoOrbPickup : MonoBehaviour
    {
        public int ammoAmount = 30;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Find a Weapon on the player hierarchy
            var weapon = other.GetComponentInChildren<Weapon>();
            if (weapon == null) return;

            weapon.AddReserveAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}
