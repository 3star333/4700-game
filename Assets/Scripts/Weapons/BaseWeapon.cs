using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Weapon";
    public float damage = 10f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    public int currentAmmo = 30;

    protected float nextFireTime = 0f;
    [Header("Visuals")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Transform firePoint;
    protected Camera playerCamera;

    public virtual void Start()
    {
        currentAmmo = maxAmmo;
        playerCamera = Camera.main;
    }

    public virtual void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            Fire();
    }

    public virtual void Fire()
    {
        // Implementation in subclasses
    }

    public virtual void Reload()
    {
        currentAmmo = maxAmmo;
    }

    public virtual void ApplyPackAPunch(float damageMultiplier = 2f)
    {
        damage *= damageMultiplier;
    }

    public virtual void ModifyDamage(float multiplier)
    {
        damage *= multiplier;
    }

    public virtual bool CanFire()
    {
        return Time.time >= nextFireTime && currentAmmo > 0;
    }

    // For UI and other systems
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
}
