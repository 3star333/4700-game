using UnityEngine;
using GothicShooter.Health;

/// <summary>
/// Enemy health component - REFACTORED to use HealthComponent + IDamageable.
/// Handles enemy-specific logic: point rewards, death VFX, round scaling.
/// Attach to any enemy GameObject alongside HealthComponent.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;

    [Header("Rewards")]
    [SerializeField] private int pointsOnKill = 100;

    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        healthComponent.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        healthComponent.OnDeath -= HandleDeath;
    }

    /// <summary>
    /// Multiply max and current health by the provided scale (called by spawner/round manager).
    /// </summary>
    public void ScaleHealth(float scale)
    {
        if (scale <= 0f) return;
        
        float scaledMaxHealth = healthComponent.MaxHealth * scale;
        healthComponent.SetMaxHealth(scaledMaxHealth);
        
        // Also scale current health if enemy hasn't taken damage yet
        if (healthComponent.CurrentHealth == healthComponent.MaxHealth / scale)
        {
            healthComponent.Heal(healthComponent.MaxHealth);
        }
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} died!");

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Play death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Award points (if a ScoreManager exists)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(pointsOnKill);
        }

        // Notify round manager enemy was killed (future hook)
        // RoundManager.Instance?.OnEnemyKilled();

        // Destroy enemy
        Destroy(gameObject);
    }

    // Public accessors for compatibility with existing code
    public float GetHealthPercentage() => healthComponent.GetHealthPercentage();
    public float GetCurrentHealth() => healthComponent.CurrentHealth;
    public float GetMaxHealth() => healthComponent.MaxHealth;
}
