using UnityEngine;
using GothicShooter.Health;

namespace GothicShooter.Mobs
{
    /// <summary>
    /// Simple wolf AI: chases the player and bites when in range.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(Collider))]
    public class WolfAI : MonoBehaviour
    {
        [Header("Target & Movement")]
        public Transform player;
        public float speed = 3.5f;
        public float rotateSpeed = 10f;

        [Header("Attack Settings")]
        public float biteRange = 1.6f;
        public float biteDamage = 15f;
        public float attackCooldown = 1.2f;

        private float lastAttackTime = -999f;

        private void Start()
        {
            if (player == null && Camera.main != null)
                player = Camera.main.transform;
        }

        private void Update()
        {
            if (player == null) return;

            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            float dist = toPlayer.magnitude;

            // Rotate towards player
            if (toPlayer.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            }

            // Move towards player
            if (dist > biteRange)
            {
                transform.position += transform.forward * (speed * Time.deltaTime);
            }
            else
            {
                TryAttack();
            }
        }

        private void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;

            // Apply damage to player via HealthComponent if present
            if (player != null)
            {
                var hc = player.GetComponentInParent<HealthComponent>();
                if (hc != null)
                {
                    hc.TakeDamage(biteDamage, gameObject);
                }
            }
        }
    }
}
