using UnityEngine;

/// <summary>
/// Base class for melee weapons.
/// </summary>
public abstract class MeleeWeapon : Weapon
{
    [Header("Melee")]
    public MeleeHitbox hitbox;

    protected void DoMelee(float dmg)
    {
        if (hitbox == null) return;
        hitbox.PerformMelee(dmg, (hitObj) =>
        {
            var eh = hitObj.GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(dmg);
        });
    }
}
