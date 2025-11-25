Interactable objects:
- Door: unlock and open via `IInteractable.Interact()` on player interaction. Costs points.
- PackAPunch: spend points to upgrade the player's weapon.
- PerkStation: spend points to gain perks like double damage.
- WeaponBuyStation: buy weapons.

How to use:
- Add a `PlayerInteractor` to your player, ensure `Player Input` has an `Interact` action that uses `PlayerInteractor.OnInteract`.
- Add `ScoreManager` and create a `PointsUI` for the UI.
