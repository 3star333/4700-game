using System.Collections;
using UnityEngine;

public class BleedEffect : MonoBehaviour
{
    private float tickAmount = 1f;
    private float duration = 3f;
    private float elapsed = 0f;
    private float tickInterval = 1f;

    private EnemyHealth enemyHealth;
    private Coroutine bleedRoutine;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
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
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(tickAmount);
            }
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(this);
    }
}
