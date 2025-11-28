using UnityEngine;
using UnityEngine.InputSystem;
using GothicShooter.Core;

/// <summary>
/// Pump-action shotgun, fires pellets in a spread.
/// REFACTORED: Now uses IDamageable interface.
/// </summary>
public class PumpShotgun : RangedWeapon
{
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 10f;
    [SerializeField] private float range = 30f; // optional override (RangedWeapon already has range)

    public override void Start()
    {
        base.Start();
    }

    public override void Fire()
    {
        if (!CanFire()) return;
        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        for (int i = 0; i < pelletCount; i++)
        {
            // Random spread
            Vector3 shootDir = playerCamera.transform.forward;
            float randX = Random.Range(-spreadAngle, spreadAngle);
            float randY = Random.Range(-spreadAngle, spreadAngle);
            shootDir = Quaternion.Euler(randY, randX, 0) * shootDir;

            if (Physics.Raycast(playerCamera.transform.position, shootDir, out RaycastHit hit, range))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage / pelletCount, gameObject);
                }
                if (impactEffect != null)
                {
                    GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }
            }
        }

        if (muzzleFlash) muzzleFlash.Play();
    }

    public override void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) Fire();
    }
}
