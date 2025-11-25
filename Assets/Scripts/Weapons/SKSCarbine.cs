using UnityEngine;

/// <summary>
/// SKS Carbine - semi-auto, good for medium-long range.
/// </summary>
public class SKSCarbine : WeaponController
{
    private void Reset()
    {
        weaponName = "SKS Carbine";
        damage = 45f;
        fireRate = 0.25f; // Semi-auto rate
        maxAmmo = 10;
        currentAmmo = maxAmmo;
        isAutomatic = false; // semi-auto
        range = 300f;
    }
}
