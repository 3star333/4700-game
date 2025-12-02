using UnityEngine;
using GothicShooter.Health;

namespace GothicShooter.Mobs
{
    /// <summary>
    /// Spawns pickup orbs (health/ammo) when an enemy with a HealthComponent dies.
    /// Attach to enemy prefabs that should drop items.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyDeathDrop : MonoBehaviour
    {
        [Header("Drop Prefabs")]
        [Tooltip("Health orb pickup prefab to spawn on death.")]
        public GameObject healthOrbPrefab;

        [Tooltip("Ammo orb pickup prefab to spawn on death.")]
        public GameObject ammoOrbPrefab;

        [Header("Drop Chances")] 
        [Range(0f, 1f)] public float healthDropChance = 0.2f;
        [Range(0f, 1f)] public float ammoDropChance = 0.3f;

        [Header("Spawn Settings")]
        public Vector3 spawnOffset = Vector3.up * 0.5f;

        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            if (_health != null)
            {
                _health.OnDeath += HandleDeath;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDeath -= HandleDeath;
            }
        }

        private void HandleDeath()
        {
            Vector3 spawnPos = transform.position + spawnOffset;

            // Health orb
            if (healthOrbPrefab != null && Random.value < healthDropChance)
            {
                Instantiate(healthOrbPrefab, spawnPos, Quaternion.identity);
            }

            // Ammo orb
            if (ammoOrbPrefab != null && Random.value < ammoDropChance)
            {
                Instantiate(ammoOrbPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
