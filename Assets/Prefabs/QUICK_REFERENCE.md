# Quick Reference - Prefab Settings

## 🔫 WEAPON PREFAB QUICK REFERENCE

### Ranged Weapons

| Weapon | Damage | Fire Rate | Range | Magazine | Reload Time |
|--------|--------|-----------|-------|----------|-------------|
| **AK101** | 35 | 600 rpm | 100m | 30 | 2.5s |
| **Mac10** | 20 | 1000 rpm | 50m | 32 | 2.0s |
| **Pump Shotgun** | 15/pellet (8 pellets) | 60 rpm | 30m | 8 | 0.5s/shell |
| **Sawed-Off** | 18/pellet (10 pellets) | 120 rpm | 15m | 2 | 2.0s |
| **SKS Carbine** | 60 | 300 rpm | 200m | 20 | 2.8s |

### Melee Weapons

| Weapon | Damage | Attack Speed | Range | Special |
|--------|--------|--------------|-------|---------|
| **Bat with Nails** | 50 | 1.5/sec | 2.5m | 30% bleed chance |
| **Longsword** | 75 | 1.0/sec | 3.0m | Heavy attack x2 |
| **Stake** | 40 | 2.0/sec | 1.5m | x3 vs vampires |

---

## 🎮 INTERACTABLE PREFAB QUICK REFERENCE

### Perk Stations

| Perk | Type | Cost | Multiplier | Light Color |
|------|------|------|------------|-------------|
| **Double Tap** | DoubleDamage | 500 | 1.5 | Purple/Magenta |
| **Speed Boost** | SpeedBoost | 500 | 1.3 | Green |
| **Extra Health** | ExtraHealth | 500 | 1.5 | Red |

**Common Settings:**
- Trigger Radius: 3m
- Use Trigger Zone: ✓
- Purchased: false

---

### Pack-a-Punch

**Settings:**
- Cost: 1000
- Damage Multiplier: 2.0
- Trigger Radius: 3m
- Light Color: Orange/Bronze

---

### Weapon Buy Stations

| Station | Weapon | Cost | Prefab Reference |
|---------|--------|------|------------------|
| **Buy_AK101** | AK-101 Assault Rifle | 1200 | AK101.prefab |
| **Buy_Mac10** | Mac-10 SMG | 800 | Mac10.prefab |
| **Buy_PumpShotgun** | Pump Shotgun | 1000 | PumpShotgun.prefab |
| **Buy_SawedOff** | Sawed-Off Shotgun | 900 | SawedOffShotgun.prefab |
| **Buy_SKS** | SKS Carbine | 1500 | SKSCarbine.prefab |

**Common Settings:**
- Trigger Radius: 3m
- Use Trigger Zone: ✓
- Light Color: Green

---

### Mystery Chests

**Standard Mystery Chest:**
- Cost: 950
- Trigger Radius: 3m
- Open Only Once: ✓
- Light Color: Gold/Yellow

**Free Mystery Chest:**
- Cost: 0
- All other settings same

**Weapon Distribution:**
```
Common (60% weight):
  - Mac10
  - BatWithNails
  - Stake
  - PumpShotgun

Uncommon (40% weight):
  - AK101
  - SawedOffShotgun

Rare (20% weight):
  - SKSCarbine
  - Longsword

Legendary (7% weight):
  - [Your legendary weapon]
```

---

## 📁 FOLDER ORGANIZATION

```
Assets/Prefabs/
│
├── Weapons/
│   ├── Guns/
│   │   ├── AK101.prefab
│   │   ├── Mac10.prefab
│   │   ├── PumpShotgun.prefab
│   │   ├── SawedOffShotgun.prefab
│   │   └── SKSCarbine.prefab
│   │
│   └── Melee/
│       ├── BatWithNails.prefab
│       ├── Longsword.prefab
│       └── Stake.prefab
│
├── Interactables/
│   ├── PerkStations/
│   │   ├── PerkStation_DoubleTap.prefab
│   │   ├── PerkStation_SpeedBoost.prefab
│   │   └── PerkStation_ExtraHealth.prefab
│   │
│   ├── PackAPunch/
│   │   └── PackAPunch.prefab
│   │
│   ├── WeaponBuyStations/
│   │   ├── WeaponBuy_AK101.prefab
│   │   ├── WeaponBuy_Mac10.prefab
│   │   ├── WeaponBuy_PumpShotgun.prefab
│   │   ├── WeaponBuy_SawedOffShotgun.prefab
│   │   └── WeaponBuy_SKSCarbine.prefab
│   │
│   └── MysteryChests/
│       ├── MysteryChest.prefab
│       └── MysteryChest_Free.prefab
│
└── UI/
    └── Panels/
        ├── PerkStationPanel.prefab
        ├── PackAPunchPanel.prefab
        ├── WeaponBuyPanel.prefab
        └── MysteryChestPanel.prefab
```

---

## ⚡ COMPONENT REQUIREMENTS

### All Weapons Must Have:
- [ ] Weapon script component (specific type)
- [ ] Audio Source
- [ ] 3D Model (optional but recommended)

### Ranged Weapons Also Need:
- [ ] Muzzle Flash Point (empty GameObject child)

### Melee Weapons Also Need:
- [ ] Hitbox Point (GameObject with MeleeHitbox component)
- [ ] Collider on hitbox (Sphere or Box)

### All Interactables Must Have:
- [ ] Interactable script (PerkStation/PackAPunch/etc.)
- [ ] SphereCollider (Is Trigger = true)
- [ ] 3D Model or visual representation

### Interactables Should Have:
- [ ] Lighting (colored light for atmosphere)
- [ ] Interaction Point (empty GameObject marker)

---

## 🎨 VISUAL GUIDELINES

### Light Colors by Type

```
Perk Stations:
  - Double Tap: #FF00FF (Magenta)
  - Speed Boost: #00FF00 (Green)
  - Extra Health: #FF0000 (Red)

Pack-a-Punch: #FF8C00 (Orange)

Weapon Buy: #00FF00 (Green)

Mystery Chest: #FFD700 (Gold)
```

### Gizmo Colors (Scene View)

```
Perk Station: Purple sphere
Pack-a-Punch: Orange sphere
Weapon Buy: Green sphere
Mystery Chest: Yellow sphere
All: 3m radius when selected
```

---

## 🔨 PREFAB CREATION WORKFLOW

### For Each Weapon:
1. Create Empty GameObject → Name it
2. Add weapon script component
3. Configure stats (see tables above)
4. Add Audio Source
5. Add 3D model as child (optional)
6. Add special components (muzzle point / hitbox)
7. Test in scene
8. Drag to Prefabs folder
9. Delete from Hierarchy

**Time per weapon:** ~5-10 minutes

---

### For Each Interactable:
1. Create Empty GameObject → Name it
2. Add interactable script
3. Configure settings (cost, type, etc.)
4. Add SphereCollider (auto-created by script)
5. Add 3D model
6. Add lighting (Point Light, colored)
7. Add interaction point marker
8. Test trigger zone in scene
9. Drag to Prefabs folder
10. Delete from Hierarchy

**Time per interactable:** ~10-15 minutes

---

## 🧪 TESTING CHECKLIST

### Weapon Test:
- [ ] Can equip weapon
- [ ] Firing/swinging works
- [ ] Damage is applied
- [ ] Ammo/stamina depletes
- [ ] Reload/cooldown works
- [ ] Audio plays
- [ ] Visual effects show

### Interactable Test:
- [ ] Trigger zone activates UI
- [ ] UI shows correct info
- [ ] Purchase/interaction works
- [ ] Points are deducted
- [ ] Effect is applied
- [ ] Can't use twice (if single-use)
- [ ] Visual feedback appears

---

## 💾 SAVING PREFABS

### Method 1: Drag to Project Window
1. Select GameObject in Hierarchy
2. Drag to Assets/Prefabs/[folder]
3. GameObject turns blue (prefab instance)
4. Delete from Hierarchy

### Method 2: Right-Click Menu
1. Right-click GameObject in Hierarchy
2. Create > Prefab
3. Choose location
4. Save

### Method 3: Assets Menu
1. Select GameObject
2. Assets > Create > Prefab
3. Choose location
4. Save

---

## 🔄 UPDATING PREFABS

### Update All Instances:
1. Select prefab in Project
2. Open in Prefab Mode (double-click)
3. Make changes
4. Save (File > Save or Ctrl+S)
5. All instances update automatically

### Update Single Instance:
1. Select instance in Hierarchy
2. Make changes
3. Right-click > Prefab > Apply Changes to Prefab

---

## 📊 PREFAB PRIORITY ORDER

If you're creating them one at a time, do in this order:

### Phase 1 - Essential Weapons (Start here)
1. Mac10 (simplest gun)
2. PumpShotgun
3. BatWithNails (simplest melee)

### Phase 2 - Core Interactables
4. MysteryChest (most important)
5. PackAPunch
6. PerkStation_DoubleTap

### Phase 3 - More Weapons
7. AK101
8. SKSCarbine
9. Longsword

### Phase 4 - Buy Stations
10. WeaponBuy_Mac10
11. WeaponBuy_PumpShotgun
12. WeaponBuy_AK101

### Phase 5 - Complete Set
13. Remaining weapons
14. Remaining buy stations
15. Remaining perk stations

---

## 🎯 COMMON PREFAB VALUES

### Trigger Radius Guide
- Close range (doors): 2m
- Standard interactables: 3m
- Large machines: 4m
- Area triggers: 5m+

### Cost Balancing
- Cheap weapons: 500-800
- Mid-tier weapons: 1000-1200
- Expensive weapons: 1500+
- Perks: 500
- Pack-a-Punch: 1000
- Mystery Chest: 950

### Damage Balancing
- Light melee: 30-50
- Heavy melee: 60-80
- SMG: 15-25
- Assault Rifle: 30-40
- Shotgun: 12-20 per pellet
- Sniper/Heavy: 50-70

---

## 🐛 TROUBLESHOOTING

**Prefab has missing references:**
→ Don't reference scene objects in prefabs
→ Use serialized fields that get assigned per-instance

**Trigger not working:**
→ Ensure SphereCollider has Is Trigger = true
→ Check trigger radius is large enough
→ Verify player has tag or QuakeMovement

**Weapon doesn't work when spawned:**
→ Check all required components are on prefab
→ Verify weapon script is assigned
→ Test prefab directly in scene first

**Interactable UI not showing:**
→ UI Controller should NOT be set in prefab
→ Assign UI Controller per scene instance
→ Check InteractableUIManager exists in scene

---

**Ready to create prefabs! Follow the PREFAB_CREATION_GUIDE.md for detailed steps. 🎮**
