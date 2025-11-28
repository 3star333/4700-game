using UnityEngine;

/// <summary>
/// Simple component for the player to enable blocking which reduces incoming damage.
/// Longsword will set the multiplier and toggle blocking state via input.
/// </summary>
public class PlayerDefense : MonoBehaviour
{
    private bool isBlocking = false;
    private float blockMultiplier = 0.5f; // remaining damage multiplier when blocking

    public void SetBlockMultiplier(float multiplier)
    {
        blockMultiplier = Mathf.Clamp01(multiplier);
    }

    public void SetBlocking(bool blocking)
    {
        isBlocking = blocking;
    }

    public float ApplyDefense(float incomingDamage)
    {
        if (isBlocking)
            return incomingDamage * blockMultiplier;
        return incomingDamage;
    }
}
