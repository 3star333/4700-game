using UnityEngine;
using GothicShooter.Health;

/// <summary>
/// Demo button/controller for testing HealthComponent: damage & heal hotkeys.
/// </summary>
public class HealthButton : MonoBehaviour
{
    [SerializeField] private HealthComponent targetHealth; // Assign player or dummy entity
    [SerializeField] private Vector2 damageRange = new Vector2(5f, 15f);
    [SerializeField] private Vector2 healRange = new Vector2(5f, 15f);

    private void Awake()
    {
        if (targetHealth == null)
        {
            targetHealth = FindObjectOfType<HealthComponent>();
        }
    }

    public void Damage()
    {
        if (targetHealth == null) return;
        float dmg = Random.Range(damageRange.x, damageRange.y);
        targetHealth.TakeDamage(dmg);
    }

    public void Heal()
    {
        if (targetHealth == null) return;
        float heal = Random.Range(healRange.x, healRange.y);
        targetHealth.Heal(heal);
    }

    private void Update()
    {
        if (targetHealth == null) return;

        // PRESS "B" TO HEAL USING KEYBOARD
        if (Input.GetKeyDown(KeyCode.B))
        {
            Heal();
        }

        // OPTIONAL DAMAGE KEY: PRESS "N"
        if (Input.GetKeyDown(KeyCode.N))
        {
            Damage();
        }
    }
}
