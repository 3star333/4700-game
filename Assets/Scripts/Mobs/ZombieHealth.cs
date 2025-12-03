using UnityEngine;
using GothicShooter.Core;
using GothicShooter.Health;

// Implements IDamageable via HealthComponent on the same GameObject
namespace GothicShooter.Mobs
{
public class ZombieHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int pointsOnKill = 50;
    [SerializeField] private HealthComponent health;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        ScoreManager.Instance?.AddPoints(pointsOnKill);
        Destroy(gameObject);
    }

    // IDamageable implementation proxies to HealthComponent
    public void TakeDamage(float amount, GameObject source = null)
    {
        if (health != null)
            health.TakeDamage(amount, source);
    }

    public bool IsDead => health != null && health.IsDead;
    public float CurrentHealth => health != null ? health.CurrentHealth : 0f;
    public float MaxHealth => health != null ? health.MaxHealth : 0f;
    public Transform Transform => transform;
}
}
