using UnityEngine;

/// <summary>
/// AK-101 - slow firing but high damage hitscan rifle.
/// </summary>
public class AK101 : Weapon
{
    public override void Start()
    {
        weaponName = "AK-101";
        damage = 65f;
        fireRate = 0.7f;
        maxAmmo = 30;
        isAutomatic = true;
        range = 200f;
        pelletsPerShot = 1;
        spreadAngle = 0f;

        base.Start();
    }
}
