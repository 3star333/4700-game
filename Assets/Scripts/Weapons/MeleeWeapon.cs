using UnityEngine;
using GothicShooter.Core;

/// <summary>
/// Base class for melee weapons.
/// REFACTORED: Now uses IDamageable interface for universal damage application.
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
            // Universal damage application via IDamageable interface
            IDamageable damageable = hitObj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(dmg, gameObject);
            }
        });
    }
}
