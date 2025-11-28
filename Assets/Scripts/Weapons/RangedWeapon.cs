using UnityEngine;

/// <summary>
/// Base class for hitscan / ranged weapons. Provides common hitscan helpers.
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

    protected void DoHitscan(Vector3 origin, Vector3 direction, float dmg, float maxRange)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRange))
        {
            var enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(dmg);
            }

            if (impactEffect != null)
            {
                var impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }
}
