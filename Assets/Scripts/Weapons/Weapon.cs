using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Root abstract Weapon class. Provides common fields and API for all weapons.
/// Other weapon types should inherit from this (RangedWeapon, ProjectileWeapon, MeleeWeapon).
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Weapon";
    public float damage = 10f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    [HideInInspector] public int currentAmmo = 0;

    protected float nextFireTime = 0f;

    public virtual void Start()
    {
        currentAmmo = maxAmmo;
    }

    public abstract void Fire();

    // Input System callback helper - subclasses can override if needed
    public virtual void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            Fire();
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

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
}
