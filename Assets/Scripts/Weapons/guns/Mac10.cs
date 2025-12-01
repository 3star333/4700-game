using UnityEngine;

/// <summary>
/// MAC-10 SMG - automatic, fast fire rate, low damage.
/// </summary>
public class Mac10 : Weapon
{
    public override void Start()
    {
        weaponName = "MAC-10";
        damage = 12f;
        fireRate = 0.08f;
        maxAmmo = 50;
        isAutomatic = true;
        range = 80f;
        pelletsPerShot = 1;
        spreadAngle = 2f;

        base.Start();
    }
}
