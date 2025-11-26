using UnityEngine;

public class Stake : BaseWeapon
{
    [Header("Melee Settings")]
    public MeleeHitbox hitbox;
    [Tooltip("Multiplier applied to enemies tagged 'Vampire'")]
    public float vampireMultiplier = 3f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (hitbox == null) return;

        hitbox.PerformMelee(damage, (hitObj) =>
        {
            var eh = hitObj.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                float finalDamage = damage;
                if (hitObj.CompareTag("Vampire"))
                {
                    finalDamage *= vampireMultiplier;
                }

                eh.TakeDamage(finalDamage);
            }
        });
    }
}
