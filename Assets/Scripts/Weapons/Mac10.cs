using UnityEngine;

/// <summary>
/// MAC-10 SMG - automatic, fast fire rate, low damage.
/// Inherits the hitscan WeaponController for simplicity.
/// Set default values so it works without additional configuration.
/// </summary>
public class Mac10 : WeaponController
{
    private void Reset()
    {
        weaponName = "MAC-10";
        damage = 12f;
        fireRate = 0.08f; // Fast
        maxAmmo = 50;
        currentAmmo = maxAmmo;
        isAutomatic = true;
        range = 80f;
    }
}
