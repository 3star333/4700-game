using System.Collections;
using UnityEngine;
using GothicShooter.Core;
using GothicShooter.Health;

/// <summary>
/// Damage over time effect that applies bleed damage.
/// REFACTORED: Now uses IDamageable and HealthComponent.
/// </summary>
public class BleedEffect : MonoBehaviour
{
    private float tickAmount = 1f;
    private float duration = 3f;
    private float elapsed = 0f;
    private float tickInterval = 1f;

    private IDamageable damageable;
    private Coroutine bleedRoutine;

    private void Awake()
    {
        damageable = GetComponent<IDamageable>();
    }

    public void StartBleed(float tick, float totalDuration)
    {
        tickAmount = tick;
        duration = totalDuration;

        if (bleedRoutine != null) StopCoroutine(bleedRoutine);
        bleedRoutine = StartCoroutine(BleedCoroutine());
    }

    private IEnumerator BleedCoroutine()
    {
        elapsed = 0f;
        while (elapsed < duration)
        {
            if (damageable != null && !damageable.IsDead)
            {
                damageable.TakeDamage(tickAmount, gameObject);
            }
            else
            {
                // Stop bleeding if target is dead
                break;
            }
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(this);
    }
}
