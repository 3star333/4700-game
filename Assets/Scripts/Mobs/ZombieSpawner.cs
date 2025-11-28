using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;   // Drag your zombie prefab here
    public Transform player;          // Drag your player here
    public float spawnRadius = 20f;   // Distance from player to spawn
    public int zombiesPerWave = 5;    // fallback number of zombies per wave
    public float timeBetweenSpawns = 1f; // Delay between spawns

    private int zombiesSpawned = 0;
    private int zombiesToSpawn = 0;
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

        // Pick random spawn position around player (keeps Y as player Y)
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(player.position.x + randomPos.x, player.position.y, player.position.z + randomPos.y);

        // Spawn zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        // Assign player dynamically and apply scaling
        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.player = player;
            ai.speed *= spawnSpeedScale;
        }

        EnemyHealth eh = zombie.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.ScaleHealth(spawnHealthScale);
        }

        // If the zombie has a damage-dealing component, try to scale it (optional)
        var dmg = zombie.GetComponent<ZombieDamage>();
        if (dmg != null)
        {
            dmg.ScaleDamage(spawnDamageScale);
        }

        zombiesSpawned++;
    }
}
