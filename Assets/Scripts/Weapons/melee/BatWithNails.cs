using System.Collections;
using UnityEngine;
using GothicShooter.Core;

public class BatWithNails : MeleeWeapon
{
    [Header("Melee Settings")]
    public MeleeHitbox hitbox;
    public float bleedTick = 2f;
    public float bleedDuration = 6f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (hitbox == null) return;
        hitbox.PerformMelee(damage, (hitObj) =>
        {
            IDamageable damageable = hitObj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, gameObject);
                
                // Apply bleed effect to the target
                var bleed = hitObj.GetComponent<BleedEffect>();
                if (bleed == null)
                    bleed = hitObj.AddComponent<BleedEffect>();

                bleed.StartBleed(bleedTick, bleedDuration);
            }
        });
    }
}
