using UnityEngine;

/// <summary>
/// Pump-action shotgun, fires pellets in a spread.
/// </summary>
public class PumpShotgun : Weapon
{
    public override void Start()
    {
        weaponName = "Pump Shotgun";
        damage = 80f; // total damage split across pellets
        fireRate = 0.8f;
        maxAmmo = 8;
        isAutomatic = false;
        range = 30f;
        pelletsPerShot = 8;
        spreadAngle = 10f;

        base.Start();
    }
}
