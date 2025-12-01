using UnityEngine;

/// <summary>
/// SKS Carbine - semi-auto, good for medium-long range.
/// </summary>
public class SKSCarbine : Weapon
{
    public override void Start()
    {
        weaponName = "SKS Carbine";
        damage = 45f;
        fireRate = 0.25f;
        maxAmmo = 10;
        isAutomatic = false;
        range = 300f;
        pelletsPerShot = 1;
        spreadAngle = 0f;

        base.Start();
    }
}
