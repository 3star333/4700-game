# Perk Bolt-On Pack

This folder contains all the **new code** and **instructions** needed to add the perk system to the game without touching the rest of the project directly.

The idea is:

- You commit this whole `perk-bolt-on` folder into the repo (for example on a branch called `perk-bolt-on`).
- Teammates open the files in here, copy/paste the code into the actual Unity scripts under `Assets/Scripts/...`, and then follow the hookup steps.

Nothing in this folder lives under `Assets/`, so Unity will NOT compile it until you manually copy code into the real scripts.

---

## 1. New scripts to add to Unity

Create these scripts inside your Unity project under `Assets/Scripts/...` using the code from the files in `NewScripts/`.

### 1.1 PlayerPerkTracker.cs

**Unity path to create:**  
`Assets/Scripts/Health/PlayerPerkTracker.cs`  

Copy the entire contents of:

- `perk-bolt-on/NewScripts/PlayerPerkTracker.cs`

Attach `PlayerPerkTracker` to the **Player** GameObject (the one with `QuakeMovement`, `HealthComponent`, etc.).

### 1.2 PerkHUD.cs

**Unity path to create:**  
`Assets/Scripts/UI/PerkHUD.cs`  

Copy the entire contents of:

- `perk-bolt-on/NewScripts/PerkHUD.cs`

Attach `PerkHUD` to a UI GameObject under your in-game HUD Canvas (e.g. `PerkHUDRoot`).

---

## 2. Changes to existing scripts (patches)

Below are patch-style snippets for existing scripts. You do **not** drop these .cs files directly into Unity; you open your existing scripts and make the changes described.

### 2.1 PerkStation.cs

**Unity path:**  
`Assets/Scripts/Interactable/PerkStation.cs`

1. At the top of the file, add:

```csharp
using GothicShooter.Health;   // for HealthComponent
```

2. Replace the entire `Interact(GameObject interactor)` method with:

```csharp
public bool Interact(GameObject interactor)
{
    if (purchased)
        return false;

    if (ScoreManager.Instance == null)
        return false;

    // Attempt to spend points first
    if (!ScoreManager.Instance.SpendPoints(cost))
    {
        Debug.Log("[PerkStation] Not enough points to buy perk.");
        return false;
    }

    bool applied = false;

    // Apply perk effect based on type
    switch (type)
    {
        case PerkType.DoubleDamage:
        {
            Weapon weapon = interactor.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.ModifyDamage(multiplier);
                applied = true;
                Debug.Log($"[PerkStation] Double Damage applied (x{multiplier})");
            }
            break;
        }
        case PerkType.SpeedBoost:
        {
            QuakeMovement qm = interactor.GetComponent<QuakeMovement>();
            if (qm != null)
            {
                qm.ApplySpeedMultiplier(multiplier);
                applied = true;
                Debug.Log($"[PerkStation] Speed Boost applied (x{multiplier})");
            }
            break;
        }
        case PerkType.ExtraHealth:
        {
            HealthComponent health = interactor.GetComponent<HealthComponent>();
            if (health != null)
            {
                float newMax = health.MaxHealth * multiplier;
                health.SetMaxHealth(newMax);
                health.FullHeal();
                applied = true;
                Debug.Log($"[PerkStation] Extra Health applied (x{multiplier})");
            }
            break;
        }
    }

    if (!applied)
    {
        // Something went wrong; refund points so the player isn't punished
        ScoreManager.Instance.AddPoints(cost);
        Debug.LogWarning("[PerkStation] Failed to apply perk: player missing component. Refunding points.");
        return false;
    }

    purchased = true;

    // Track perk for HUD/icons if tracker exists
    PlayerPerkTracker tracker = interactor.GetComponent<PlayerPerkTracker>();
    if (tracker != null)
    {
        tracker.RegisterPerk(type);
    }

    return true;
}
```

---

### 2.2 PerkStationUI.cs

**Unity path:**  
`Assets/Scripts/UI/PerkStationUI.cs`

Find the existing `GetPerkDescription(PerkType type)` method (it has `[STUB]` text) and replace it with:

```csharp
private string GetPerkDescription(PerkType type)
{
    switch (type)
    {
        case PerkType.DoubleDamage:
            return "Increases your weapon damage.\nStacks with other damage modifiers.";
        case PerkType.SpeedBoost:
            return "Increases your base movement speed.\nAffects walking, strafing, and sliding.";
        case PerkType.ExtraHealth:
            return "Increases your maximum health and heals you to full.";
        default:
            return "Perk effect not defined.";
    }
}
```

---

### 2.3 QuakeMovement.cs

**Unity path:**  
`Assets/Scripts/QuakeMovement.cs`

1. Near the top, under movement settings, add the multiplier field:

```csharp
[Header("Movement Settings")]
[SerializeField] private float baseGroundSpeed = 7f;
[SerializeField] private float meleeWeaponSpeedBonus = 0.15f; // 15% speed boost for melee

[Tooltip("Global movement speed multiplier applied by perks like SpeedBoost.")]
[SerializeField] private float movementSpeedMultiplier = 1f;
```

(You already have the first two lines; just add `movementSpeedMultiplier` + tooltip.)

2. Add this public method inside the `QuakeMovement` class:

```csharp
/// <summary>
/// Apply a multiplicative speed boost (e.g. 1.5f = +50% speed).
/// Called by PerkStation SpeedBoost.
/// </summary>
public void ApplySpeedMultiplier(float multiplier)
{
    if (multiplier <= 0f) return;
    movementSpeedMultiplier *= multiplier;
}
```

3. Update `GetGroundSpeed()` to use the multiplier:

Find:

```csharp
private float GetGroundSpeed()
{
    float speed = baseGroundSpeed;

    // Apply melee weapon speed bonus
    if (hasMeleeWeapon)
    {
        speed *= (1f + meleeWeaponSpeedBonus);
    }

    return speed;
}
```

Replace with:

```csharp
private float GetGroundSpeed()
{
    float speed = baseGroundSpeed * movementSpeedMultiplier;

    // Apply melee weapon speed bonus
    if (hasMeleeWeapon)
    {
        speed *= (1f + meleeWeaponSpeedBonus);
    }

    return speed;
}
```

4. Make slide speed respect the perk multiplier:

In your slide start logic (inside `StartSlide()` or equivalent), find:

```csharp
if (horizontalVelocity.magnitude < 0.1f)
{
    slideDirection = transform.forward;
    currentSlideSpeed = baseGroundSpeed;
}
```

Change the assignment to:

```csharp
    slideDirection = transform.forward;
    currentSlideSpeed = baseGroundSpeed * movementSpeedMultiplier;
```

---

## 3. Hookup steps in Unity (for teammates)

Once the code changes above are made in the **real** Unity scripts:

### 3.1 Player setup

1. Select the **Player** GameObject.
2. Add **`PlayerPerkTracker`** component.
3. Ensure the Player also has:
   - `QuakeMovement`
   - `HealthComponent`
   - A `Weapon` component in its children (so `GetComponentInChildren<Weapon>()` works).

### 3.2 Perk HUD setup

1. In the main HUD Canvas, create an empty UI GameObject (e.g. `PerkHUDRoot`).
2. Add **`PerkHUD`** component to it.
3. Set in the inspector:
   - `iconContainer` → the `PerkHUDRoot` transform.
   - `perkIconPrefab` → a prefab that contains an `Image` for the perk icon.
4. In the `PerkIcons` list:
   - Add an entry for each `PerkType` you use:
     - `DoubleDamage` → damage icon sprite.
     - `SpeedBoost` → speed icon sprite.
     - `ExtraHealth` → health icon sprite.

### 3.3 PerkStation setup

1. Make sure each perk machine has a `PerkStation` component.
2. Configure:
   - `type` → `DoubleDamage`, `SpeedBoost`, or `ExtraHealth`.
   - `cost` → points cost.
   - `multiplier` → effect strength (e.g. `2.0` for DoubleDamage).
3. Make sure there is a `ScoreManager` in the scene (`ScoreManager.Instance` must exist).

### 3.4 Quick test

1. Start the game, earn enough points.
2. Open a PerkStation UI and buy a perk.
3. Verify:
   - DoubleDamage → enemies die faster.
   - SpeedBoost → player movement/slide is faster.
   - ExtraHealth → max health increases and player is fully healed.
   - PerkHUD → shows an icon for each acquired perk.

---

## 4. Suggested git workflow (for this folder)

1. Drop this `perk-bolt-on` folder into the **root** of the repo (NOT under `Assets/`).
2. Commit it on a new branch:

```bash
git checkout main
git pull origin main
git checkout -b perk-bolt-on
git add perk-bolt-on
git commit -m "Add perk bolt-on pack (docs + scripts)"
git push -u origin perk-bolt-on
```

3. Teammates can then:

```bash
git fetch
git checkout perk-bolt-on
```

…and follow the steps in this README and the `NewScripts` folder.
