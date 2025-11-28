using System.Collections;
using UnityEngine;

/// <summary>
/// Controls round progression, calculates how many zombies to spawn each round,
/// and scales enemy stats based on round number. Exposes tuning sliders in the Inspector.
/// </summary>
public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int startingZombies = 6;
    public float maxZombiesPerMap = 120f; // soft cap for map size
    public float roundLengthSeconds = 60f; // optional automatic round time

    [Header("Spawn Scaling")]
    [Range(0f, 2f)] public float zombieCountMultiplierPerRound = 1.15f; // multiplicative growth
    [Range(0f, 1f)] public float percentOfMapToFill = 0.15f; // fraction of maxZombiesPerMap to aim for

    [Header("Stat Scaling")]
    [Range(0f, 3f)] public float healthMultiplierPerRound = 1.07f;
    [Range(0f, 3f)] public float speedMultiplierPerRound = 1.02f;
    [Range(0f, 3f)] public float damageMultiplierPerRound = 1.03f;

    public ZombieSpawner spawner;

    private int currentRound = 0;
    private int zombiesThisRound = 0;

    private void Start()
    {
        if (spawner == null)
            spawner = FindObjectOfType<ZombieSpawner>();

        StartNextRound();
    }

    public void StartNextRound()
    {
        currentRound++;
        // Calculate target zombies for this round
        float baseCount = startingZombies * Mathf.Pow(zombieCountMultiplierPerRound, currentRound - 1);
        float mapCap = Mathf.Max(1f, maxZombiesPerMap * percentOfMapToFill);
        int target = Mathf.Clamp(Mathf.RoundToInt(baseCount), 1, Mathf.RoundToInt(mapCap));

        zombiesThisRound = target;

        // Tell spawner to start spawning this many zombies and provide scaling factors
        if (spawner != null)
        {
            float healthScale = Mathf.Pow(healthMultiplierPerRound, currentRound - 1);
            float speedScale = Mathf.Pow(speedMultiplierPerRound, currentRound - 1);
            float damageScale = Mathf.Pow(damageMultiplierPerRound, currentRound - 1);

            spawner.StartWave(zombiesThisRound, healthScale, speedScale, damageScale);
        }
    }

    // Optional helper for UI
    public int GetCurrentRound() => currentRound;
    public int GetZombiesThisRound() => zombiesThisRound;
}
