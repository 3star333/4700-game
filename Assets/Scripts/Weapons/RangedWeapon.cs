using UnityEngine;
using GothicShooter.Core;

/// <summary>
/// Base class for hitscan / ranged weapons. Provides common hitscan helpers.
/// REFACTORED: Now uses IDamageable interface for universal damage application.
/// </summary>
public abstract class RangedWeapon : Weapon
{
    [Header("Ranged")]
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Transform firePoint;
    protected Camera playerCamera;

    public override void Start()
    {
        base.Start();
        playerCamera = Camera.main;
    }

    /// <summary>
    /// Perform hitscan raycast and apply damage to any IDamageable entity.
    /// Works with enemies, destructibles, props, players (for friendly fire), etc.
    /// </summary>
    protected void DoHitscan(Vector3 origin, Vector3 direction, float dmg, float maxRange)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRange))
        {
            // Universal damage application via IDamageable interface
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(dmg, gameObject);
            }

            // Spawn impact effect at hit point
            if (impactEffect != null)
            {
                var impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}
