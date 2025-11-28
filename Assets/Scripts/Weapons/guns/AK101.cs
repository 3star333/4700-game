using UnityEngine;

/// <summary>
/// AK-101 - slow firing but high damage hitscan rifle.
/// </summary>
public class AK101 : WeaponController
{
    private void Reset()
    {
        weaponName = "AK-101";
        damage = 65f;
        fireRate = 0.7f;
        maxAmmo = 30;
        currentAmmo = maxAmmo;
        isAutomatic = true;
        range = 200f;
    }
}
