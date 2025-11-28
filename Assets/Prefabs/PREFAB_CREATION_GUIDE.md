# Prefab Creation Guide - Weapons & Interactables

## Overview

This guide provides step-by-step instructions for creating prefabs for all weapons and interactables in your gothic wave-survival shooter project.

---

## 📁 Folder Structure to Create

First, create this folder structure in your Prefabs folder:

```
Assets/Prefabs/
├── Weapons/
│   ├── Guns/
│   └── Melee/
├── Interactables/
│   ├── PerkStations/
│   ├── PackAPunch/
│   ├── WeaponBuyStations/
│   └── MysteryChests/
└── UI/
    └── Panels/
```

**To create folders:**
1. Right-click on `Assets/Prefabs`
2. Create > Folder
3. Name each folder as shown above

---

## 🔫 WEAPON PREFABS

### Ranged Weapons (Guns)

#### 1. AK101 Assault Rifle

**Location:** `Assets/Prefabs/Weapons/Guns/AK101.prefab`

**Steps:**
1. Create empty GameObject: Hierarchy > Right-click > Create Empty
2. Name it: `AK101`
3. Add Component: `AK101` script (Scripts/Weapons/guns/AK101.cs)
4. Configure in Inspector:
   ```
   Damage: 35
   Fire Rate: 600 (rounds per minute)
   Range: 100
   Accuracy: 0.95
   Magazine Size: 30
   Reload Time: 2.5
   ```
5. Add 3D Model (optional):
   - Drag AK model from Low Poly Weapons pack as child
   - Position at (0, 0, 0) relative to parent
6. Add Audio Source component (for gunshot sounds)
7. Add Muzzle Flash Point:
   - Create empty child GameObject: `MuzzleFlashPoint`
   - Position at barrel tip
8. Drag GameObject from Hierarchy to `Assets/Prefabs/Weapons/Guns/`
9. Delete from Hierarchy (now it's a prefab)

---

#### 2. Mac10 SMG

**Location:** `Assets/Prefabs/Weapons/Guns/Mac10.prefab`

**Steps:**
1. Create empty GameObject: `Mac10`
2. Add Component: `Mac10` script
3. Configure:
   ```
   Damage: 20
   Fire Rate: 1000
   Range: 50
   Accuracy: 0.75
   Magazine Size: 32
   Reload Time: 2.0
   ```
4. Add 3D Model (from Low Poly Weapons)
5. Add Audio Source
6. Add Muzzle Flash Point
7. Save as prefab in `Guns/`

---

#### 3. Pump Shotgun

**Location:** `Assets/Prefabs/Weapons/Guns/PumpShotgun.prefab`

**Steps:**
1. Create empty GameObject: `PumpShotgun`
2. Add Component: `PumpShotgun` script
3. Configure:
   ```
   Damage: 15 (per pellet)
   Pellet Count: 8
   Fire Rate: 60 (pump action)
   Range: 30
   Spread: 0.15
   Magazine Size: 8
   Reload Time: 0.5 (per shell)
   ```
4. Add 3D Model
5. Add Audio Source
6. Add Muzzle Flash Point
7. Save as prefab in `Guns/`

---

#### 4. Sawed-Off Shotgun

**Location:** `Assets/Prefabs/Weapons/Guns/SawedOffShotgun.prefab`

**Steps:**
1. Create empty GameObject: `SawedOffShotgun`
2. Add Component: `SawedOffShotgun` script
3. Configure:
   ```
   Damage: 18 (per pellet)
   Pellet Count: 10
   Fire Rate: 120
   Range: 15
   Spread: 0.25
   Magazine Size: 2
   Reload Time: 2.0
   ```
4. Add 3D Model (shorter shotgun model)
5. Add Audio Source
6. Add Muzzle Flash Point
7. Save as prefab in `Guns/`

---

#### 5. SKS Carbine

**Location:** `Assets/Prefabs/Weapons/Guns/SKSCarbine.prefab`

**Steps:**
1. Create empty GameObject: `SKSCarbine`
2. Add Component: `SKSCarbine` script
3. Configure:
   ```
   Damage: 60
   Fire Rate: 300
   Range: 200
   Accuracy: 0.98
   Magazine Size: 20
   Reload Time: 2.8
   ```
4. Add 3D Model (rifle model)
5. Add Audio Source
6. Add Muzzle Flash Point
7. Save as prefab in `Guns/`

---

### Melee Weapons

#### 6. Bat With Nails

**Location:** `Assets/Prefabs/Weapons/Melee/BatWithNails.prefab`

**Steps:**
1. Create empty GameObject: `BatWithNails`
2. Add Component: `BatWithNails` script
3. Configure:
   ```
   Damage: 50
   Attack Speed: 1.5 (attacks per second)
   Range: 2.5
   Bleed Chance: 0.3
   Bleed Damage: 5
   Bleed Duration: 3
   ```
4. Add 3D Model (bat with nails)
5. Add child GameObject: `HitboxPoint`
   - Position at tip of bat
   - Add `MeleeHitbox` component
   - Set Collider: Sphere, radius 0.5
6. Add Audio Source (for swing sounds)
7. Save as prefab in `Melee/`

---

#### 7. Longsword

**Location:** `Assets/Prefabs/Weapons/Melee/Longsword.prefab`

**Steps:**
1. Create empty GameObject: `Longsword`
2. Add Component: `Longsword` script
3. Configure:
   ```
   Damage: 75
   Attack Speed: 1.0
   Range: 3.0
   Heavy Attack Multiplier: 2.0
   Stamina Cost: 10
   ```
4. Add 3D Model (sword model)
5. Add child GameObject: `HitboxPoint`
   - Position at middle of blade
   - Add `MeleeHitbox` component
   - Set Collider: Box, size appropriate to blade
6. Add Audio Source
7. Save as prefab in `Melee/`

---

#### 8. Stake

**Location:** `Assets/Prefabs/Weapons/Melee/Stake.prefab`

**Steps:**
1. Create empty GameObject: `Stake`
2. Add Component: `Stake` script
3. Configure:
   ```
   Damage: 40
   Attack Speed: 2.0
   Range: 1.5
   Vampire Damage Multiplier: 3.0
   ```
4. Add 3D Model (wooden stake)
5. Add child GameObject: `HitboxPoint`
   - Position at tip
   - Add `MeleeHitbox` component
6. Add Audio Source
7. Save as prefab in `Melee/`

---

## 🎮 INTERACTABLE PREFABS

### Perk Stations

#### 9. Perk Station - Double Tap

**Location:** `Assets/Prefabs/Interactables/PerkStations/PerkStation_DoubleTap.prefab`

**Steps:**
1. Create empty GameObject: `PerkStation_DoubleTap`
2. Add Component: `PerkStation` script
3. Configure:
   ```
   [Perk Settings]
   Type: DoubleDamage
   Cost: 500
   Multiplier: 1.5
   Purchased: false
   
   [UI Reference]
   UI Controller: [Leave empty, assign at runtime]
   
   [Trigger Settings]
   Use Trigger Zone: ✓
   Trigger Radius: 3
   ```
4. Add 3D Model/Visual:
   - Use vending machine or pedestal model
   - Add colored light (purple/magenta)
5. Add SphereCollider (auto-created by script):
   - Is Trigger: ✓
   - Radius: 3
6. Add child: Empty GameObject named `InteractionPoint`
7. Save as prefab in `PerkStations/`

---

#### 10. Perk Station - Speed Boost

**Location:** `Assets/Prefabs/Interactables/PerkStations/PerkStation_SpeedBoost.prefab`

**Steps:**
1. Duplicate `PerkStation_DoubleTap` prefab
2. Rename to: `PerkStation_SpeedBoost`
3. Modify `PerkStation` component:
   ```
   Type: SpeedBoost
   Cost: 500
   Multiplier: 1.3
   ```
4. Change light color to green
5. Save

---

#### 11. Perk Station - Extra Health

**Location:** `Assets/Prefabs/Interactables/PerkStations/PerkStation_ExtraHealth.prefab`

**Steps:**
1. Duplicate `PerkStation_DoubleTap` prefab
2. Rename to: `PerkStation_ExtraHealth`
3. Modify `PerkStation` component:
   ```
   Type: ExtraHealth
   Cost: 500
   Multiplier: 1.5
   ```
4. Change light color to red
5. Save

---

### Pack-a-Punch Machine

#### 12. Pack-a-Punch

**Location:** `Assets/Prefabs/Interactables/PackAPunch/PackAPunch.prefab`

**Steps:**
1. Create empty GameObject: `PackAPunch`
2. Add Component: `PackAPunch` script
3. Configure:
   ```
   [Pack-a-Punch Settings]
   Cost: 1000
   Damage Multiplier: 2.0
   Used: false
   
   [UI Reference]
   UI Controller: [Leave empty]
   
   [Trigger Settings]
   Use Trigger Zone: ✓
   Trigger Radius: 3
   ```
4. Add 3D Model:
   - Large machine/altar model
   - Add orange/bronze lighting
   - Add particle effect (optional)
5. Add SphereCollider (auto-created)
6. Add child: `WeaponSlot` (where weapon appears during upgrade)
7. Save as prefab in `PackAPunch/`

---

### Weapon Buy Stations

#### 13. Weapon Buy - AK101

**Location:** `Assets/Prefabs/Interactables/WeaponBuyStations/WeaponBuy_AK101.prefab`

**Steps:**
1. Create empty GameObject: `WeaponBuy_AK101`
2. Add Component: `WeaponBuyStation` script
3. Configure:
   ```
   [Weapon Settings]
   Weapon Name: "AK-101 Assault Rifle"
   Cost: 1200
   Weapon Prefab: [Drag AK101 prefab here]
   Purchased: false
   
   [UI Reference]
   UI Controller: [Leave empty]
   
   [Trigger Settings]
   Use Trigger Zone: ✓
   Trigger Radius: 3
   ```
4. Add Visual:
   - Wall mount or display case
   - Show weapon model (for visual reference)
   - Add green light
5. Add SphereCollider
6. Save as prefab in `WeaponBuyStations/`

---

#### 14. Weapon Buy - Mac10

**Location:** `Assets/Prefabs/Interactables/WeaponBuyStations/WeaponBuy_Mac10.prefab`

**Steps:**
1. Duplicate `WeaponBuy_AK101`
2. Rename to: `WeaponBuy_Mac10`
3. Configure:
   ```
   Weapon Name: "Mac-10 SMG"
   Cost: 800
   Weapon Prefab: [Mac10 prefab]
   ```
4. Update visual model
5. Save

---

#### 15. Weapon Buy - Pump Shotgun

**Steps:** (Same as above)
```
Name: "Pump Shotgun"
Cost: 1000
Prefab: PumpShotgun
```

---

#### 16. Weapon Buy - Sawed-Off Shotgun

**Steps:** (Same as above)
```
Name: "Sawed-Off Shotgun"
Cost: 900
Prefab: SawedOffShotgun
```

---

#### 17. Weapon Buy - SKS Carbine

**Steps:** (Same as above)
```
Name: "SKS Carbine"
Cost: 1500
Prefab: SKSCarbine
```

---

### Mystery Chests

#### 18. Mystery Chest

**Location:** `Assets/Prefabs/Interactables/MysteryChests/MysteryChest.prefab`

**Steps:**
1. Create empty GameObject: `MysteryChest`
2. Add Component: `LootChest` script
3. Configure:
   ```
   [Spawn Settings]
   Loot Spawn Point: [Create child, assign below]
   Open Only Once: ✓
   
   [Weapon Prefabs By Rarity]
   Common Weapons (4):
     - Mac10
     - BatWithNails
     - Stake
     - PumpShotgun
   
   Uncommon Weapons (2):
     - AK101
     - SawedOffShotgun
   
   Rare Weapons (2):
     - SKSCarbine
     - Longsword
   
   Legendary Weapons (1):
     - (Add your legendary weapon when created)
   
   [Rarity Weights]
   Legendary Weight: 7
   Rare Weight: 20
   Uncommon Weight: 40
   Common Weight: 60
   
   [Optional Cost]
   Cost: 950
   
   [UI Reference]
   UI Controller: [Leave empty]
   
   [Trigger Settings]
   Use Trigger Zone: ✓
   Trigger Radius: 3
   ```
4. Add 3D Model:
   - Use chest model from "Chest assets" folder
   - Add golden/yellow light
   - Add particle effect
5. Add child GameObject: `LootSpawnPoint`
   - Position 1 unit above chest
6. Add SphereCollider
7. Save as prefab in `MysteryChests/`

---

#### 19. Mystery Chest - Free Version

**Location:** `Assets/Prefabs/Interactables/MysteryChests/MysteryChest_Free.prefab`

**Steps:**
1. Duplicate `MysteryChest` prefab
2. Rename to: `MysteryChest_Free`
3. Change Cost to: `0`
4. Save

---

## 🎨 UI PANEL PREFABS

### 20. Perk Station Panel

**Location:** `Assets/Prefabs/UI/Panels/PerkStationPanel.prefab`

**Steps:**
1. Open any scene with your InteractableCanvas
2. Select `PerkStationPanel` from Canvas
3. Drag it to `Assets/Prefabs/UI/Panels/`
4. You now have a reusable panel prefab
5. Can instantiate in any scene

---

### 21. Pack-a-Punch Panel

**Steps:** Same as above for `PackAPunchPanel`

---

### 22. Weapon Buy Panel

**Steps:** Same as above for `WeaponBuyPanel`

---

### 23. Mystery Chest Panel

**Steps:** Same as above for `MysteryChestPanel`

---

## ✅ PREFAB CREATION CHECKLIST

### Weapons - Guns (5)
- [ ] AK101.prefab
- [ ] Mac10.prefab
- [ ] PumpShotgun.prefab
- [ ] SawedOffShotgun.prefab
- [ ] SKSCarbine.prefab

### Weapons - Melee (3)
- [ ] BatWithNails.prefab
- [ ] Longsword.prefab
- [ ] Stake.prefab

### Interactables - Perk Stations (3)
- [ ] PerkStation_DoubleTap.prefab
- [ ] PerkStation_SpeedBoost.prefab
- [ ] PerkStation_ExtraHealth.prefab

### Interactables - Pack-a-Punch (1)
- [ ] PackAPunch.prefab

### Interactables - Weapon Buy Stations (5)
- [ ] WeaponBuy_AK101.prefab
- [ ] WeaponBuy_Mac10.prefab
- [ ] WeaponBuy_PumpShotgun.prefab
- [ ] WeaponBuy_SawedOffShotgun.prefab
- [ ] WeaponBuy_SKSCarbine.prefab

### Interactables - Mystery Chests (2)
- [ ] MysteryChest.prefab
- [ ] MysteryChest_Free.prefab

### UI Panels (4)
- [ ] PerkStationPanel.prefab
- [ ] PackAPunchPanel.prefab
- [ ] WeaponBuyPanel.prefab
- [ ] MysteryChestPanel.prefab

**Total: 23 Prefabs**

---

## 🔧 TIPS FOR PREFAB CREATION

### Best Practices

1. **Name Consistently**: Use clear, descriptive names
2. **Organize by Type**: Keep similar prefabs in same folder
3. **Test Before Saving**: Verify component settings work
4. **Use Prefab Variants**: For similar items (like multiple perk stations)
5. **Document Settings**: Add comments in inspector for important values

### Common Mistakes to Avoid

❌ Don't forget to add colliders to interactables
❌ Don't leave required references empty
❌ Don't save scene-specific references in prefabs
❌ Don't forget to set trigger zones

✅ Do test each prefab in a test scene
✅ Do verify all scripts are assigned
✅ Do use consistent scaling (usually 1,1,1)
✅ Do add visual gizmos for debugging

---

## 🎯 USING YOUR PREFABS

### To Place in Scene:
1. Drag prefab from Project window to Hierarchy
2. Position in Scene view
3. (For interactables) Wire up UI controller reference
4. Test interaction

### To Reference in Code:
```csharp
[SerializeField] private GameObject weaponPrefab; // Drag prefab here
```

### To Instantiate at Runtime:
```csharp
GameObject instance = Instantiate(weaponPrefab, position, rotation);
```

---

## 📦 PREFAB VARIANTS (Advanced)

For similar items, create variants:

1. Right-click existing prefab
2. Create > Prefab Variant
3. Modify settings
4. Save with new name

**Example:**
- Base: `PerkStation.prefab`
- Variant 1: `PerkStation_DoubleTap.prefab`
- Variant 2: `PerkStation_SpeedBoost.prefab`

---

## 🐛 TROUBLESHOOTING

**Prefab Won't Save:**
- Ensure you're dragging from Hierarchy to Project window
- Check you have write permissions in Prefabs folder

**Missing Script References:**
- Verify all scripts are compiled without errors
- Check script file names match class names

**Prefab Looks Different in Scene:**
- Check for scene-specific lighting
- Verify materials are assigned
- Check scale is uniform (1,1,1)

**Interactable UI Not Working:**
- Don't assign UI Controller in prefab
- Assign it per-instance in each scene
- UI Controller is scene-specific, not prefab-stored

---

## 🚀 QUICK START WORKFLOW

1. **Create one weapon** (start with Mac10 - simplest)
2. **Test it** in a test scene
3. **Save as prefab**
4. **Use as template** for other weapons
5. **Repeat** for interactables

**Estimated Time:**
- All weapon prefabs: 1-2 hours
- All interactable prefabs: 1-2 hours
- UI panel prefabs: 15 minutes

---

## 📝 FINAL NOTES

- Prefabs are reusable across all scenes
- Changes to prefab affect all instances
- Use "Prefab > Unpack" to make instance unique
- Keep prefabs organized by type
- Document any custom settings

**Your prefab library is now ready! 🎮**
