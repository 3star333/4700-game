using UnityEngine;

public class SawedOffShotgun : PumpShotgun
{
    [Header("Sawed Off Settings")]
    public float recoilForce = 10f; // Strong pushback

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
