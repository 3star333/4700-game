using UnityEngine;

public class HealthButton : MonoBehaviour
{
    [SerializeField] private Health health;

    public void Damage()
    {
        float dmg = Random.Range(5f, 15f);
        health.TakeDamage(dmg);
    }

    public void Heal()
    {
        float heal = Random.Range(5f, 15f);
        health.Heal(heal);
    }

    private void Update()
    {
        // PRESS "B" TO HEAL USING KEYBOARD
        if (Input.GetKeyDown(KeyCode.B))
        {
            Heal();
        }

        // OPTIONAL DAMAGE KEY: PRESS "N"
        // if (Input.GetKeyDown(KeyCode.N))
        // {
        //     Damage();
        // }
    }
}
