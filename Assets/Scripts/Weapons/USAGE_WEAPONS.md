Usage guide for the refactored weapon system

Overview
- The weapon system has been refactored into an OOP hierarchy:
  - `Weapon` (root): common stats and API (damage, fireRate, ammo, CanFire, Reload).
  - `RangedWeapon`: hitscan-style weapons (range, muzzle flash, impact effect, firePoint).
  - `ProjectileWeapon`: spawns projectile prefabs and applies force.
  - `MeleeWeapon`: melee-specific helper using `MeleeHitbox`.
- `WeaponManager` manages a player's collection of `Weapon` instances and handles equip/switch.
- Every gun now uses its own dedicated script (e.g., `AK101`, `Mac10`, `PumpShotgun`) so each prefab can carry unique stats, models, audio, and logic.

Folder quick reference (Assets/Scripts/Weapons)
- `Weapon.cs`, `RangedWeapon.cs`, `ProjectileWeapon.cs`, `MeleeWeapon.cs` – base classes.
- `WeaponController.cs` – generic hitscan weapon logic used by `AK101`, `Mac10`, `SKSCarbine`.
- `PumpShotgun.cs`, `SawedOffShotgun.cs` – shotgun implementations.
- `Longsword.cs`, `Stake.cs`, `BatWithNails.cs` – melee weapons.
- `WeaponManager.cs` – attaches to the Player and handles owning/equipping weapons.

Quick steps to create and attach a weapon in Unity
1. Create a weapon prefab
  - Create an empty GameObject (e.g., `AK101_Prefab`) and child GameObjects for `FirePoint`/`ProjectileSpawn` as needed.
  - Add the dedicated weapon script for that gun:
    - Hitscan rifles/SMGs/snipers: use `WeaponController`-based scripts such as `AK101`, `Mac10`, `SKSCarbine`.
    - Shotguns: use `PumpShotgun` or `SawedOffShotgun` (inherits `RangedWeapon`).
    - Projectile weapons: create a script deriving from `ProjectileWeapon` or duplicate an existing projectile gun template.
    - Melee: `Longsword`, `Stake`, `BatWithNails`.
  - Set the component fields in the Inspector: `damage`, `fireRate`, `range`, `muzzleFlash`, `impactEffect`, `firePoint`, `projectilePrefab`, `projectileSpawn`, `AudioSource`, etc.
  - Save the GameObject as a prefab in `Assets/Prefabs/Weapons`.

2. Equip to the player
  - Ensure the Player GameObject has a `WeaponManager` component and its `weaponMount` set to a child Transform where prefabs attach.
  - At runtime call `WeaponManager.AddWeapon(yourWeaponPrefab)` or place weapon prefabs under the `weaponMount` in the scene hierarchy and add via inspector.

Notes & migration tips
- Existing weapon prefabs that used the old `BaseWeapon` should be updated to use the new classes. Most concrete weapon scripts have been updated already (e.g., `WeaponController`, `PumpShotgun`, `Longsword`).
- `BaseWeapon.cs` and `Gun.cs` are deprecated compatibility shims; remove them from prefabs when convenient.
- Input callbacks: weapon classes provide `OnFire(InputAction.CallbackContext)` virtual method; your Input System setup can call `OnFire` on the equipped weapon.
- Perks and interactables now query for `Weapon` instead of `BaseWeapon` — `PerkStation` and `PackAPunch` were updated.

Common tasks
- Create a fast hitscan rifle:
  - Add `WeaponController` to a prefab, set `damage`, `fireRate`, `range`, `muzzleFlash`, and `firePoint`.
- Create a shotgun:
  - Use `PumpShotgun` prefab and adjust `pelletCount` and `spreadAngle`.
- Create a projectile gun:
  - Derive a new script from `ProjectileWeapon`, assign `projectilePrefab`, `projectileSpawn`, and `projectileForce`.

Attaching a gun to the player (step-by-step)
1. Prepare the weapon prefab
  - Duplicate one of the existing prefabs in `Assets/Prefabs/Weapons` or create a new empty GameObject for your gun model.
  - Attach the dedicated script for that gun (e.g., `AK101`, `Mac10`, `PumpShotgun`). These scripts inherit `Weapon`, so they already expose `damage`, `fireRate`, `range`, etc. in the inspector.
  - Add child transforms for `FirePoint`, `ProjectileSpawn`, and any muzzle-flash VFX origins. Assign those references on the script component.
  - Add supporting components: `AudioSource` (with shoot/empty clips), particle systems, animator references, etc.
  - Save the configured GameObject as a prefab.

2. Mount the weapon on the player
  - On the Player GameObject ensure a `WeaponManager` component exists and that its `weaponMount` field points to a child transform where guns should visually attach (e.g., a hand bone or a socket under the camera).
  - Easiest setup: parent your FPS camera to the player (as QuakeMovement already does) and add an empty child under the camera called `WeaponMount`. Assign that Transform to `WeaponManager.weaponMount`. Now any weapon added through `WeaponManager.AddWeapon` automatically spawns under the camera and follows its rotation.
  - In code (e.g., in a setup script or pickup interaction) call `weaponManager.AddWeapon(prefabReference)` to instantiate and attach the gun, or pre-place the prefab under the `weaponMount` and call `weaponManager.AddOwnedWeapon(existingWeaponComponent)`.
  - `WeaponManager.EquipWeapon(index)` (or `EquipNextWeapon()`) toggles active weapons; only the equipped gun stays enabled.

3. Hook up input
  - The equipped weapon exposes `OnFire(InputAction.CallbackContext)`. Your player input script can simply forward the `Fire` action to the currently equipped weapon: `weaponManager.CurrentWeapon?.OnFire(context);`.
  - If your gun script handles held-fire vs single-shot (e.g., `WeaponController` uses an `isAutomatic` flag), no extra logic is needed.
  - For reloading, add another input action that calls `weaponManager.CurrentWeapon?.Reload();`.

4. Audio, VFX, and animation per gun
  - Because each gun has its own script/prefab, you can tailor mechanics (recoil, spread, camera kick, reload animation triggers) directly inside that script without affecting others.
  - Mesh/model + material setup happens on the prefab; `WeaponManager` only instantiates and toggles GameObjects.

Testing tips
- Enter Play Mode, ensure the Player has a `WeaponManager` with at least one weapon added (some scenes already place a gun under the mount). Verify left click (or your bound fire action) triggers the gun’s `OnFire` and ammo decreases.
- Use the `WeaponManager` inspector buttons (optional) to switch weapons or trigger `ApplyPackAPunchToCurrent` and confirm damage scaling.
- If nothing happens when firing, check that `WeaponManager.CurrentWeapon` is not null and that the weapon script’s `CanFire()` is returning true (ammo > 0, respecting `fireRate`).
