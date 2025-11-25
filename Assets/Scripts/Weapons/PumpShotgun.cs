using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pump-action shotgun, fires pellets in a spread.
/// </summary>
public class PumpShotgun : BaseWeapon
{
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 10f;
    [SerializeField] private float range = 30f;

    private Camera playerCamera;

    public override void Start()
    {
        base.Start();
        playerCamera = Camera.main;
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
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage / pelletCount);
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
