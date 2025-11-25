This folder contains zombie and enemy related scripts.
- `ZombieAI.cs` - Simple AI that moves toward the player.
- `ZombieHealth.cs` - Health for a zombie and awards points on death.
- `ZombieSpawner.cs` - Spawner for the zombies (a wave spawner).
- `EnemyHealth.cs` - Generic enemy health that awards points on death.

Notes:
- Attach these scripts to their respective prefabs or within scene objects.
- `EnemyHealth` and `ZombieHealth` both award points via `ScoreManager.Instance.AddPoints()`; modify `pointsOnKill` for desired reward amounts.
