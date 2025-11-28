// Assets/Scripts/Health/PlayerHealth.cs
using UnityEngine;
using GothicShooter.Core;

namespace GothicShooter.Health
{
    /// <summary>
    /// Player-specific health wrapper.
    /// Adds player-only features: shield system, damage feedback, game over trigger.
    /// Listens to HealthComponent events and adds player-specific responses.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Shield System (Future Feature)")]
        [SerializeField] private float maxShield = 0f;
        [SerializeField] private float shieldRegenRate = 5f;
        [SerializeField] private float shieldRegenDelay = 3f;

        [Header("Damage Feedback")]
        [SerializeField] private bool enableScreenShake = true;
        [SerializeField] private float damageShakeIntensity = 0.2f;
        [SerializeField] private float damageShakeDuration = 0.15f;

        private HealthComponent healthComponent;
        private float currentShield;
        private float timeSinceLastDamage;
        private bool isDead = false;

        // Public properties for UI
        public float CurrentHealth => healthComponent.CurrentHealth;
        public float MaxHealth => healthComponent.MaxHealth;
        public float CurrentShield => currentShield;
        public float MaxShield => maxShield;

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            currentShield = maxShield;
        }

        private void OnEnable()
        {
            healthComponent.OnDamageTaken += HandleDamageTaken;
            healthComponent.OnDeath += HandleDeath;
            healthComponent.OnHealed += HandleHealed;
        }

        private void OnDisable()
        {
            healthComponent.OnDamageTaken -= HandleDamageTaken;
            healthComponent.OnDeath -= HandleDeath;
            healthComponent.OnHealed -= HandleHealed;
        }

        private void Update()
        {
            if (isDead) return;

            RegenerateShield();
        }

        private void RegenerateShield()
        {
            // Shield regeneration system (stub for future implementation)
            if (maxShield > 0 && currentShield < maxShield)
            {
                timeSinceLastDamage += Time.deltaTime;
                
                if (timeSinceLastDamage >= shieldRegenDelay)
                {
                    currentShield += shieldRegenRate * Time.deltaTime;
                    currentShield = Mathf.Min(currentShield, maxShield);
                }
            }
        }

        private void HandleDamageTaken(float damageAmount, float newHealth, GameObject damageSource)
        {
            timeSinceLastDamage = 0f;

            // Screen shake feedback
            if (enableScreenShake)
            {
                // TODO: Hook up CameraShake when implemented
                // CameraShake.Instance?.Shake(damageShakeIntensity, damageShakeDuration);
                Debug.Log($"[PlayerHealth] Screen shake: {damageShakeIntensity} for {damageShakeDuration}s");
            }

            // Audio feedback
            // TODO: Hook up AudioManager when implemented
            // AudioManager.Instance?.PlayPlayerHurt();
            Debug.Log($"[PlayerHealth] Player took {damageAmount} damage. Health: {newHealth}/{healthComponent.MaxHealth}");

            // Screen flash effect
            // TODO: Hook up ScreenFlash when implemented
            // ScreenFlash.Instance?.Flash(Color.red, 0.2f);
        }

        private void HandleHealed(float healAmount)
        {
            // Audio feedback for healing
            // TODO: Hook up AudioManager when implemented
            // AudioManager.Instance?.PlayHealSound();
            Debug.Log($"[PlayerHealth] Player healed {healAmount}. Health: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
        }

        private void HandleDeath()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("[PlayerHealth] Player has died!");

            // Trigger Game Over state
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionToGameOver();
            }

            // Disable player controls
            DisablePlayerControls();

            // Play death animation
            // TODO: Hook up animation when implemented
            // GetComponent<Animator>()?.SetTrigger("Death");

            // Audio feedback
            // TODO: Hook up AudioManager when implemented
            // AudioManager.Instance?.PlayPlayerDeath();

            // Drop weapon, disable input, etc.
            // TODO: Implement weapon drop system
        }

        private void DisablePlayerControls()
        {
            // Disable movement
            var movement = GetComponent<QuakeMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }

            // Disable weapon input
            var weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.enabled = false;
            }

            // Lock cursor for UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Public methods for UI
        public float GetHealthPercentage()
        {
            return healthComponent.GetHealthPercentage();
        }

        public float GetShieldPercentage()
        {
            return maxShield > 0 ? currentShield / maxShield : 0f;
        }

        // Public methods for perks/powerups
        public void AddShield(float amount)
        {
            currentShield += amount;
            currentShield = Mathf.Min(currentShield, maxShield);
        }

        public void SetMaxShield(float newMax)
        {
            maxShield = newMax;
            currentShield = Mathf.Min(currentShield, maxShield);
        }

        // Editor utilities
        [ContextMenu("Take 25 Damage")]
        private void DebugTakeDamage()
        {
            healthComponent.TakeDamage(25f);
        }

        [ContextMenu("Heal to Full")]
        private void DebugHeal()
        {
            healthComponent.FullHeal();
        }
    }
}
