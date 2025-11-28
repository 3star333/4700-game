using UnityEngine;

/// <summary>
/// Base class for projectile-based weapons. Handles creating projectiles.
/// </summary>
public abstract class ProjectileWeapon : Weapon
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public float projectileForce = 20f;

    protected void SpawnProjectile()
    {
        if (projectilePrefab == null || projectileSpawn == null) return;
        var proj = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(projectileSpawn.forward * projectileForce, ForceMode.Impulse);
    }
}
