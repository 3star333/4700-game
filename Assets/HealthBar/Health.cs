using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    public UnityEvent<float> onHealthChanged;
    public UnityEvent onDeath;

    private bool isDead = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        onHealthChanged?.Invoke(CurrentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - Mathf.Abs(amount), 0f, maxHealth);
        onHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            isDead = true;
            onDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + Mathf.Abs(amount), 0f, maxHealth);
        onHealthChanged?.Invoke(CurrentHealth);
    }

    public float GetMaxHealth() => maxHealth;
}
