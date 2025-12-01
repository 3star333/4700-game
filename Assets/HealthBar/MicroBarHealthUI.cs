using UnityEngine;
using UnityEngine.UI;
using GothicShooter.Health;

/// <summary>
/// Micro health bar UI that now uses <see cref="HealthComponent"/> directly instead of the old Health script.
/// Subscribes to HealthComponent events (OnDamageTaken / OnHealed / OnDeath) and drives a single fill Image.
/// </summary>
public class MicroBarHealthUI : MonoBehaviour
{
    [SerializeField] private HealthComponent healthComponent; // Optional manual assign; auto-found if null
    [SerializeField] private Image fillImage; // Set Image.type = Filled in Inspector
    [SerializeField] private bool invertFill;

    private float maxHealth = 1f;

    private void Awake()
    {
        // Auto-find if not wired in inspector
        if (healthComponent == null)
        {
            healthComponent = FindObjectOfType<HealthComponent>();
        }
    }

    private void OnEnable()
    {
        if (!Validate()) return;
        CacheMax();
        UpdateBarImmediate();
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private bool Validate()
    {
        if (healthComponent == null)
        {
            Debug.LogWarning("MicroBarHealthUI: HealthComponent missing.");
            enabled = false;
            return false;
        }
        if (fillImage == null)
        {
            Debug.LogWarning("MicroBarHealthUI: fillImage not assigned.");
            enabled = false;
            return false;
        }
        return true;
    }

    private void CacheMax()
    {
        maxHealth = Mathf.Max(1f, healthComponent.MaxHealth);
    }

    private void Subscribe()
    {
        healthComponent.OnDamageTaken += HandleDamageTaken;
        healthComponent.OnHealed += HandleHealed;
        healthComponent.OnDeath += HandleDeath;
    }

    private void Unsubscribe()
    {
        if (healthComponent == null) return;
        healthComponent.OnDamageTaken -= HandleDamageTaken;
        healthComponent.OnHealed -= HandleHealed;
        healthComponent.OnDeath -= HandleDeath;
    }

    private void HandleDamageTaken(float dmg, float newHealth, GameObject src)
    {
        UpdateBar(newHealth);
    }

    private void HandleHealed(float healAmount)
    {
        UpdateBar(healthComponent.CurrentHealth);
    }

    private void HandleDeath()
    {
        UpdateBar(0f);
    }

    private void UpdateBarImmediate()
    {
        UpdateBar(healthComponent.CurrentHealth);
    }

    private void UpdateBar(float healthValue)
    {
        float pct = Mathf.Clamp01(healthValue / maxHealth);
        if (invertFill) pct = 1f - pct;
        fillImage.fillAmount = pct;
    }
}
