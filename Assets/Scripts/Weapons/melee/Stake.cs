using UnityEngine;
using GothicShooter.Core;

public class Stake : MeleeWeapon
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
        // Custom vampire multiplier handling
        hitbox.PerformMelee(damage, (hitObj) =>
        {
            IDamageable damageable = hitObj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float finalDamage = damage;
                if (hitObj.CompareTag("Vampire"))
                {
                    finalDamage *= vampireMultiplier;
                }

                damageable.TakeDamage(finalDamage, gameObject);
            }
        });
    }
}
