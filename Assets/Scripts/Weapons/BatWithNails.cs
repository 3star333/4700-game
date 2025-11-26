using System.Collections;
using UnityEngine;

public class BatWithNails : BaseWeapon
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
            var eh = hitObj.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                // Apply bleed as a coroutine on the enemy if possible
                var bleed = hitObj.GetComponent<BleedEffect>();
                if (bleed == null)
                    bleed = hitObj.AddComponent<BleedEffect>();

                bleed.StartBleed(bleedTick, bleedDuration);
            }
        });
    }
}
