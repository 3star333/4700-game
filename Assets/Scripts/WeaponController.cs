using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic weapon controller for shooting mechanics.
/// Attach to the player's camera or a weapon object.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float range = 100f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Transform firePoint;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip emptySound;

    private float nextFireTime = 0f;
    private Camera playerCamera;
    private bool shootInput = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
        playerCamera = Camera.main;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Handle shooting
        if (shootInput && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        else if (shootInput && currentAmmo <= 0)
        {
            // Play empty sound
            if (audioSource && emptySound)
                audioSource.PlayOneShot(emptySound);
        }
    }

    private void Shoot()
    {
        currentAmmo--;

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

    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded!");
    }

    // Input System callbacks
    public void OnAttack(InputAction.CallbackContext context)
    {
        shootInput = context.ReadValueAsButton();
    }

    // Alternative for Send Messages behavior
    public void OnAttack(InputValue value)
    {
        shootInput = value.isPressed;
    }

    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
}
