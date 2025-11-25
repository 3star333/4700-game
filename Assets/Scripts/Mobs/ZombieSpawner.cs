using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;   // Drag your zombie prefab here
    public Transform player;          // Drag your player here
    public float spawnRadius = 20f;   // Distance from player to spawn
    public int zombiesPerWave = 5;    // Number of zombies per wave
    public float timeBetweenSpawns = 1f; // Delay between spawns

    private int zombiesSpawned = 0;

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        zombiesSpawned = 0;
        InvokeRepeating(nameof(SpawnZombie), 0f, timeBetweenSpawns);
    }

    void SpawnZombie()
    {
        if (zombiesSpawned >= zombiesPerWave)
        {
            CancelInvoke(nameof(SpawnZombie));
            return;
        }

        // Pick random spawn position around player
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(player.position.x + randomPos.x, player.position.y, player.position.z + randomPos.y);

        // Spawn zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        // Assign player dynamically
        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.player = player;
        }

        zombiesSpawned++;
    }
}
