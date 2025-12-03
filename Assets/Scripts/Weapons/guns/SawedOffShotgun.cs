using UnityEngine;

/// <summary>
/// Sawed-off shotgun with strong recoil that pushes player backward.
/// </summary>
public class SawedOffShotgun : Weapon
{
    [Header("Sawed Off Settings")]
    public float recoilForce = 10f; // Strong pushback

    public override void Start()
    {
        weaponName = "Sawed-Off Shotgun";
        damage = 60f; // Lower damage per pellet than pump
        fireRate = 0.6f; // Faster than pump
        maxAmmo = 2; // Only 2 shells
        isAutomatic = false;
        range = 20f; // Very short range
        pelletsPerShot = 10; // More pellets
        spreadAngle = 15f; // Wide spread

        base.Start();
    }

    public override void Fire()
    {
        base.Fire();

        // Apply pushback to the owner (player)
        QuakeMovement qm = GetComponentInParent<QuakeMovement>();
        if (qm != null)
        {
            // Push the player backward (opposite of forward direction)
            Vector3 backward = -transform.forward * recoilForce;
            qm.AddImpulse(backward);
        }
    }
}
