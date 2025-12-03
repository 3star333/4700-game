using UnityEngine;
using GothicShooter.Health;

namespace GothicShooter.Pickups
{
    /// <summary>
    /// Simple health pickup: heals the first HealthComponent it touches.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HealthOrbPickup : MonoBehaviour
    {
        public float healAmount = 25f;

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
            var health = other.GetComponentInChildren<HealthComponent>();
            if (health == null) return;

            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
