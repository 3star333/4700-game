Usage details for the Score/Points system:

- ScoreManager (Script):
  - Singleton that tracks points, call `ScoreManager.Instance.AddPoints(amount)` to award points.
  - Call `ScoreManager.Instance.SpendPoints(cost)` to try to spend points (returns bool success).
  - Add `ScoreManager` component to a GameObject named `GameManager` in the scene, or it will be auto-created at runtime.

- EnemyHealth / ZombieHealth:
  - `pointsOnKill` field defines how many points a kill grants.
  - When enemy dies it calls `ScoreManager.Instance.AddPoints(pointsOnKill)`.

- Points UI:
  - Use `PointsUI` script attached to a UI Text (or TextMeshProUGUI) to show current points.
  - Create a UI Canvas and place `PointsUI` on a GameObject, set `pointsText` or `pointsTextTMP` reference in the Inspector.

- Door (Interactable):
  - Use `Door` component with `cost` field; the player can interact (using PlayerInteractor) to spend points and open a door.
  - `Door` can use cost and rotate to open on success.

- Interacting:
  - Add `PlayerInteractor` to the Player (same GameObject as PlayerInput/QuakeMovement).
  - Player Input action `Interact` should be configured in `InputSystem_Actions`.
  - When input performed, `PlayerInteractor` will raycast forward from camera center and call `Interact(GameObject interactor)` on hit objects implementing `IInteractable`.

- Buy Stations / Perks / Pack-A-Punch:
  - `WeaponBuyStation`: spend points to buy a weapon prefab (instantiated as a child of player).
  - `PackAPunch`: spend points to upgrade the player's weapon with a damage multiplier.
  - `PerkStation`: spend points for perks (DoubleDamage implemented), can be extended for speed/health.

How to test quickly:
1. Add ScoreManager to your scene (or let it auto create)
2. Add `PlayerInteractor` and `WeaponController` to the Player GameObject.
3. Add `PointsUI` to a Canvas and wire the text.
4. Add a `ZombiePrefab` with `ZombieHealth` and kill it, check your points increase in UI.
5. Add a `Door` object and interact (press `E`) to buy it.
6. Add `WeaponBuyStation` / `PackAPunch` / `PerkStation` and try to interact.

Notes & TODO:
- Still needs a proper weapon switching system (weapon equip/unequip/holster).
- UI message for insufficient points and buy feedback.
- Visual & audio feedback for successful purchase.

