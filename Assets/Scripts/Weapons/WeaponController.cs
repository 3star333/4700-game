using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic weapon controller for shooting mechanics.
/// Attach to the player's camera or a weapon object.
/// </summary>
public class WeaponController : BaseWeapon
{
    [Header("Weapon Settings")]
    [SerializeField] protected float range = 100f;
    [SerializeField] public bool isAutomatic = true;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Transform firePoint;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip emptySound;

    private Camera playerCamera;
    private bool shootInput = false;

    public override void Start()
    {
        base.Start();
        playerCamera = Camera.main;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Handle shooting
        if (shootInput && CanFire())
        {
            Fire();
        }
        else if (shootInput && currentAmmo <= 0)
        {
            // Play empty sound
            if (audioSource && emptySound)
                audioSource.PlayOneShot(emptySound);
        }
    }

    public override void Fire()
    {
        if (!CanFire()) return;
        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        // Visual effects
        if (muzzleFlash)
            muzzleFlash.Play();

        // Audio
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        // Raycast to detect hit
        RaycastHit hit;
        Vector3 shootDirection = playerCamera.transform.forward;
        
        if (firePoint != null)
            shootDirection = firePoint.forward;

        if (Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, range))
        {
            Debug.Log($"Hit: {hit.collider.name}");

            // Check if we hit an enemy
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Spawn impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            // Debug line to visualize shot
            Debug.DrawRay(playerCamera.transform.position, shootDirection * hit.distance, Color.red, 1f);
        }
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded!");
    }

    // Input System callbacks
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isAutomatic)
        {
            shootInput = context.ReadValueAsButton();
        }
        else
        {
            if (context.performed) Fire();
        }
    }

    // Alternative for Send Messages behavior
    public void OnAttack(InputValue value)
    {
        shootInput = value.isPressed;
    }

    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

    // Apply a Pack-A-Punch style upgrade to this weapon (increase damage/other stats)
    public override void ApplyPackAPunch(float damageMultiplier = 2f)
    {
        base.ApplyPackAPunch(damageMultiplier);
        Debug.Log($"Weapon upgraded by Pack-A-Punch! New damage: {damage}");
    }
    public override void ModifyDamage(float multiplier)
    {
        base.ModifyDamage(multiplier);
        Debug.Log($"Weapon damage modified. New damage: {damage}");
    }
}
