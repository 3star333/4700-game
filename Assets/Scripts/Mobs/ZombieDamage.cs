using UnityEngine;

/// <summary>
/// Optional component for zombies that deals damage to player.
/// Provides a ScaleDamage method so RoundManager/Spawner can increase damage per round.
/// </summary>
public class ZombieDamage : MonoBehaviour
{
    public float damage = 10f;

    public void ScaleDamage(float scale)
    {
        if (scale <= 0f) return;
        damage *= scale;
    }
}
