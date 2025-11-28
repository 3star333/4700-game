// Assets/Scripts/Health/HealthComponent.cs
using UnityEngine;
using System;
using GothicShooter.Core;

namespace GothicShooter.Health
{
    /// <summary>
    /// Reusable health component implementing IDamageable.
    /// Attach to Player, Enemies, Destructible Props, Bosses, etc.
    /// Contains zero UI logic - only health state and events.
    /// </summary>
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool invulnerable = false;
        [SerializeField] private bool destroyOnDeath = false;

        private float currentHealth;

        // Events - UI and other systems subscribe to these
        public event Action<float, float, GameObject> OnDamageTaken; // (damageAmount, newHealth, damageSource)
        public event Action<float> OnHealed; // (healAmount)
        public event Action OnDeath;

        // IDamageable Properties
        public bool IsDead { get; private set; }
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public Transform Transform => transform;

        private void Awake()
        {
            currentHealth = maxHealth;
            IsDead = false;
        }

        /// <summary>
        /// Apply damage to this entity. Called from weapons, traps, explosions, etc.
        /// </summary>
        public void TakeDamage(float amount, GameObject source = null)
        {
            if (IsDead || invulnerable || amount <= 0) return;

            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0f);

            // Notify subscribers (UI, audio, VFX systems)
            OnDamageTaken?.Invoke(amount, currentHealth, source);

            // Check for death
            if (currentHealth <= 0f && !IsDead)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal this entity. Used by perks, pickups, regeneration, etc.
        /// </summary>
        public void Heal(float amount)
        {
            if (IsDead || amount <= 0) return;

            float previousHealth = currentHealth;
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            // Only invoke if health actually changed
            if (currentHealth > previousHealth)
            {
                OnHealed?.Invoke(amount);
            }
        }

        /// <summary>
        /// Set new max health. Used by perks like Juggernog.
        /// </summary>
        public void SetMaxHealth(float newMax)
        {
            if (newMax <= 0) return;

            maxHealth = newMax;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        /// <summary>
        /// Toggle invulnerability. Used for cutscenes, respawn protection, etc.
        /// </summary>
        public void SetInvulnerable(bool state)
        {
            invulnerable = state;
        }

        /// <summary>
        /// Instantly kill this entity.
        /// </summary>
        public void Kill()
        {
            TakeDamage(currentHealth);
        }

        /// <summary>
        /// Fully restore health.
        /// </summary>
        public void FullHeal()
        {
            Heal(maxHealth);
        }

        /// <summary>
        /// Get health as percentage (0-1). Useful for UI bars.
        /// </summary>
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0f;
        }

        private void Die()
        {
            IsDead = true;
            OnDeath?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        // Editor utilities for debugging
        [ContextMenu("Take 10 Damage")]
        private void DebugTakeDamage() => TakeDamage(10f);

        [ContextMenu("Heal 20")]
        private void DebugHeal() => Heal(20f);

        [ContextMenu("Kill")]
        private void DebugKill() => Kill();

        [ContextMenu("Full Heal")]
        private void DebugFullHeal() => FullHeal();
    }
}
