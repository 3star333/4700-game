using UnityEngine;
using GothicShooter.Health;
namespace GothicShooter.Mobs
{
public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;   // Drag your zombie prefab here
    public Transform player;          // Drag your player here
    [Tooltip("Max distance from player to spawn. Typical FPS feel ~45m")] public float spawnRadius = 45f;   // Distance from player to spawn
    [Tooltip("Min distance from player to avoid popping in their face")] public float minSpawnRadius = 10f;
    [Tooltip("Attempts to snap spawn to NavMesh if available")] public bool useNavMeshPlacement = true;
    public int zombiesPerWave = 5;    // fallback number of zombies per wave
    public float timeBetweenSpawns = 1f; // Delay between spawns

    public System.Action OnWaveCompleted; // fired when all spawned zombies are dead

    private int zombiesSpawned = 0;
    private int zombiesToSpawn = 0;
    private int zombiesAlive = 0;
    private float spawnHealthScale = 1f;
    private float spawnSpeedScale = 1f;
    private float spawnDamageScale = 1f;

    void Start()
    {
        // keep existing behavior if used directly
        if (zombiesToSpawn == 0)
            zombiesToSpawn = zombiesPerWave;

        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }

    /// <summary>
    /// Start spawning a wave with scaling applied.
    /// </summary>
    public void StartWave(int count, float healthScale, float speedScale, float damageScale)
    {
        zombiesToSpawn = Mathf.Max(1, count);
        spawnHealthScale = healthScale;
        spawnSpeedScale = speedScale;
        spawnDamageScale = damageScale;

        zombiesSpawned = 0;
    zombiesAlive = 0;
        CancelInvoke(nameof(SpawnZombie));
        InvokeRepeating(nameof(SpawnZombie), 0f, timeBetweenSpawns);
    }

    void SpawnZombie()
    {
        if (zombiesSpawned >= zombiesToSpawn)
        {
            CancelInvoke(nameof(SpawnZombie));
            return;
        }

        // Pick random spawn position around player within [minSpawnRadius, spawnRadius]
        float r = Random.Range(minSpawnRadius, spawnRadius);
        float ang = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
        Vector3 spawnPos = player.position + offset;

        // Try NavMesh placement if requested
        if (useNavMeshPlacement)
        {
            TryPlaceOnNavMesh(ref spawnPos);
        }

        // Spawn zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        // Assign player dynamically and apply scaling
        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.player = player;
            ai.speed *= spawnSpeedScale;
        }

        // Scale health via HealthComponent if present
        var hc = zombie.GetComponent<HealthComponent>();
        if (hc != null)
        {
            hc.SetMaxHealth(Mathf.RoundToInt(hc.MaxHealth * spawnHealthScale));
            hc.FullHeal();
            hc.OnDeath += HandleZombieDeath;
        }

        zombiesSpawned++;
        zombiesAlive++;

        // All spawned? If zero desired (edge) invoke complete immediately.
        if (zombiesSpawned >= zombiesToSpawn)
        {
            CancelInvoke(nameof(SpawnZombie));
        }
    }

    private void HandleZombieDeath()
    {
        zombiesAlive = Mathf.Max(0, zombiesAlive - 1);
        if (zombiesAlive == 0 && zombiesSpawned >= zombiesToSpawn)
        {
            OnWaveCompleted?.Invoke();
        }
    }

    // Attempts to place spawn on nearest valid NavMesh position, falls back to original if NavMesh not present
    private void TryPlaceOnNavMesh(ref Vector3 pos)
    {
        // Guard: NavMesh might not be referenced in this assembly unless using UnityEngine.AI
        #if UNITY_2018_3_OR_NEWER
        try
        {
            var aiNs = typeof(UnityEngine.AI.NavMesh);
            // Sample near the position
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit hit, 6f, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = hit.position;
            }
        }
        catch { /* ignore if AI namespace missing */ }
        #endif
    }
}
}
