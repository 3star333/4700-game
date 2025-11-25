using UnityEngine;

/// <summary>
/// Simple health system for enemies.
/// Attach to any enemy GameObject.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;

    [Header("Rewards")]
    [SerializeField] private int pointsOnKill = 100;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
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

        // Award points (you'll implement this when you have a score system)
        // ScoreManager.Instance?.AddPoints(pointsOnKill);

        // Destroy enemy
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}
