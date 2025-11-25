using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float health = 100f;
    [SerializeField] private int pointsOnKill = 50;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        ScoreManager.Instance?.AddPoints(pointsOnKill);
        Destroy(gameObject); // Simple death: destroy zombie
    }
}
