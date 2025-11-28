# Quick Reference: Unity Inspector Configuration

## InteractableCanvas Setup

```
InteractableCanvas (GameObject)
├─ Canvas (Component)
│  ├─ Render Mode: Screen Space - Overlay
│  ├─ Canvas Scaler
│  │  ├─ UI Scale Mode: Scale With Screen Size
│  │  ├─ Reference Resolution: 1920 x 1080
│  │  └─ Match: 0.5
│  └─ Graphic Raycaster (auto-added)
│
├─ InteractableUIManager (Component) ← ADD THIS SCRIPT
│  ├─ Perk Station Panel: [PerkStationPanel]
│  ├─ Pack A Punch Panel: [PackAPunchPanel]
│  ├─ Weapon Buy Panel: [WeaponBuyPanel]
│  ├─ Mystery Chest Panel: [MysteryChestPanel]
│  ├─ Fade Duration: 0.2
│  └─ Enable ESC To Close: ✓
│
├─ PerkStationPanel (Panel)
│  ├─ RectTransform: 600x700, Center
│  ├─ Image: rgba(20,10,30,220)
│  ├─ PerkStationUI (Component) ← ADD THIS SCRIPT
│  │  ├─ Title Text: [TitleText]
│  │  ├─ Perk Name Text: [PerkNameText]
│  │  ├─ Cost Text: [CostText]
│  │  ├─ Description Text: [DescriptionText]
│  │  ├─ Player Points Text: [PlayerPointsText]
│  │  ├─ Buy Button: [BuyButton]
│  │  ├─ Status Text: [StatusText]
│  │  └─ Perk Station: [Assigned at runtime]
│  │
│  ├─ TitleText (TextMeshProUGUI)
│  │  ├─ Text: "PERK STATION"
│  │  ├─ Font Size: 48
│  │  ├─ Color: #FFD700
│  │  └─ Alignment: Center/Top
│  │
│  ├─ PerkNameText (TextMeshProUGUI)
│  │  ├─ Font Size: 36
│  │  └─ Color: #FFFFFF
│  │
│  ├─ CostText (TextMeshProUGUI)
│  │  ├─ Font Size: 28
│  │  └─ Color: #FFD700
│  │
│  ├─ DescriptionText (TextMeshProUGUI)
│  │  ├─ Font Size: 20
│  │  ├─ Color: #CCCCCC
│  │  └─ Word Wrap: ✓
│  │
│  ├─ PlayerPointsText (TextMeshProUGUI)
│  │  ├─ Font Size: 24
│  │  └─ Color: #00FF00
│  │
│  ├─ BuyButton (Button - TextMeshProUGUI)
│  │  ├─ Image Color: rgba(100,50,150,200)
│  │  └─ Text: "PURCHASE PERK"
│  │     ├─ Font Size: 32
│  │     └─ Color: #FFFFFF
│  │
│  └─ StatusText (TextMeshProUGUI)
│     ├─ Font Size: 22
│     └─ Color: Dynamic (green/red/yellow)
│
├─ PackAPunchPanel (Panel)
│  ├─ RectTransform: 600x700, Center
│  ├─ Image: rgba(40,20,10,220)
│  ├─ PackAPunchUI (Component) ← ADD THIS SCRIPT
│  │  ├─ Title Text: [TitleText]
│  │  ├─ Weapon Name Text: [WeaponNameText]
│  │  ├─ Cost Text: [CostText]
│  │  ├─ Description Text: [DescriptionText]
│  │  ├─ Player Points Text: [PlayerPointsText]
│  │  ├─ Upgrade Button: [UpgradeButton]
│  │  ├─ Status Text: [StatusText]
│  │  ├─ Multiplier Text: [MultiplierText]
│  │  └─ Pack A Punch: [Assigned at runtime]
│  │
│  └─ [Similar child elements, orange theme]
│
├─ WeaponBuyPanel (Panel)
│  ├─ RectTransform: 600x750, Center
│  ├─ Image: rgba(10,30,20,220)
│  ├─ WeaponBuyUI (Component) ← ADD THIS SCRIPT
│  │  ├─ Title Text: [TitleText]
│  │  ├─ Weapon Name Text: [WeaponNameText]
│  │  ├─ Cost Text: [CostText]
│  │  ├─ Description Text: [DescriptionText]
│  │  ├─ Player Points Text: [PlayerPointsText]
│  │  ├─ Weapon Icon Image: [WeaponIconImage]
│  │  ├─ Buy Button: [BuyButton]
│  │  ├─ Status Text: [StatusText]
│  │  ├─ Weapon Buy Station: [Assigned at runtime]
│  │  └─ Weapon Icon: [Optional sprite]
│  │
│  ├─ WeaponIconImage (Image)
│  │  ├─ Size: 256x256
│  │  └─ Preserve Aspect: ✓
│  │
│  └─ [Similar child elements, green theme]
│
└─ MysteryChestPanel (Panel)
   ├─ RectTransform: 600x800, Center
   ├─ Image: rgba(40,40,10,220)
   ├─ MysteryChestUI (Component) ← ADD THIS SCRIPT
   │  ├─ Title Text: [TitleText]
   │  ├─ Cost Text: [CostText]
   │  ├─ Description Text: [DescriptionText]
   │  ├─ Player Points Text: [PlayerPointsText]
   │  ├─ Open Button: [OpenButton]
   │  ├─ Status Text: [StatusText]
   │  ├─ Rarity Info Text: [RarityInfoText]
   │  └─ Loot Chest: [Assigned at runtime]
   │
   ├─ RarityInfoText (TextMeshProUGUI)
   │  ├─ Font Size: 18
   │  ├─ Rich Text: ✓
   │  └─ Shows color-coded drop rates
   │
   └─ [Similar child elements, gold theme]
```

---

## Interactable GameObject Setup

### Perk Station

```
PerkStation_DoubleTap (GameObject)
├─ [Your 3D Model/Mesh]
├─ SphereCollider (Component) ← AUTO-CREATED
│  ├─ Is Trigger: ✓
│  └─ Radius: 3
├─ PerkStation (Component)
│  ├─ [Perk Settings]
│  │  ├─ Type: DoubleDamage
│  │  ├─ Cost: 500
│  │  ├─ Multiplier: 1.5
│  │  └─ Purchased: ☐
│  ├─ [UI Reference]
│  │  └─ UI Controller: [Drag PerkStationUI component]
│  └─ [Trigger Settings]
│     ├─ Use Trigger Zone: ✓
│     └─ Trigger Radius: 3
└─ (Gizmo shows purple sphere in Scene view)
```

### Pack-a-Punch

```
PackAPunch (GameObject)
├─ [Your 3D Model/Mesh]
├─ SphereCollider (Component) ← AUTO-CREATED
│  ├─ Is Trigger: ✓
│  └─ Radius: 3
├─ PackAPunch (Component)
│  ├─ [Pack-a-Punch Settings]
│  │  ├─ Cost: 1000
│  │  ├─ Damage Multiplier: 2
│  │  └─ Used: ☐
│  ├─ [UI Reference]
│  │  └─ UI Controller: [Drag PackAPunchUI component]
│  └─ [Trigger Settings]
│     ├─ Use Trigger Zone: ✓
│     └─ Trigger Radius: 3
└─ (Gizmo shows orange sphere in Scene view)
```

### Weapon Buy Station

```
WeaponBuy_AK47 (GameObject)
├─ [Your 3D Model/Mesh or Wall-mount]
├─ SphereCollider (Component) ← AUTO-CREATED
│  ├─ Is Trigger: ✓
│  └─ Radius: 3
├─ WeaponBuyStation (Component)
│  ├─ [Weapon Settings]
│  │  ├─ Weapon Name: "AK-47"
│  │  ├─ Cost: 1200
│  │  ├─ Weapon Prefab: [Drag weapon prefab]
│  │  └─ Purchased: ☐
│  ├─ [UI Reference]
│  │  └─ UI Controller: [Drag WeaponBuyUI component]
│  └─ [Trigger Settings]
│     ├─ Use Trigger Zone: ✓
│     └─ Trigger Radius: 3
└─ (Gizmo shows green sphere in Scene view)
```

### Mystery Chest

```
MysteryChest (GameObject)
├─ [Your 3D Chest Model]
├─ LootSpawnPoint (Empty GameObject)
│  └─ Position: Above chest
├─ SphereCollider (Component) ← AUTO-CREATED
│  ├─ Is Trigger: ✓
│  └─ Radius: 3
├─ LootChest (Component)
│  ├─ [Spawn Settings]
│  │  ├─ Loot Spawn Point: [LootSpawnPoint]
│  │  └─ Open Only Once: ✓
│  ├─ [Weapon Prefabs By Rarity]
│  │  ├─ Common Weapons: [Array of prefabs]
│  │  ├─ Uncommon Weapons: [Array of prefabs]
│  │  ├─ Rare Weapons: [Array of prefabs]
│  │  └─ Legendary Weapons: [Array of prefabs]
│  ├─ [Rarity Weights]
│  │  ├─ Legendary Weight: 7
│  │  ├─ Rare Weight: 20
│  │  ├─ Uncommon Weight: 40
│  │  └─ Common Weight: 60
│  ├─ [Optional Cost]
│  │  └─ Cost: 950
│  ├─ [UI Reference]
│  │  └─ UI Controller: [Drag MysteryChestUI component]
│  └─ [Trigger Settings]
│     ├─ Use Trigger Zone: ✓
│     └─ Trigger Radius: 3
└─ (Gizmo shows yellow sphere + spawn point)
```

---

## Player GameObject Setup

```
Player (GameObject)
├─ Tag: "Player" ← IMPORTANT!
├─ Capsule Collider (Component)
│  └─ Is Trigger: ☐ (must be FALSE)
├─ Rigidbody (Component)
├─ QuakeMovement (Component)
├─ PlayerInteractor (Component)
│  ├─ Interact Range: 4
│  └─ Interact Layers: Everything
├─ WeaponManager (Component)
│  └─ Weapon Mount: [Child transform]
└─ Camera (Child)
   └─ Tag: "MainCamera"
```

---

## Scene Hierarchy Example

```
Hierarchy
├─ Player
│  ├─ Main Camera
│  └─ WeaponMount
├─ Environment
│  ├─ Floor
│  ├─ Walls
│  └─ Props
├─ Interactables
│  ├─ PerkStation_DoubleTap
│  ├─ PerkStation_SpeedBoost
│  ├─ PackAPunch
│  ├─ WeaponBuy_AK47
│  ├─ WeaponBuy_Shotgun
│  └─ MysteryChest
├─ Managers
│  ├─ ScoreManager
│  └─ RoundManager
└─ UI
   ├─ InteractableCanvas ← YOUR NEW UI SYSTEM
   │  ├─ PerkStationPanel
   │  ├─ PackAPunchPanel
   │  ├─ WeaponBuyPanel
   │  └─ MysteryChestPanel
   └─ HUD
      ├─ HealthBar
      ├─ AmmoCounter
      └─ PointsDisplay
```

---

## Inspector Quick Drag Reference

### Step 1: Wire InteractableUIManager
1. Select `InteractableCanvas`
2. Find `InteractableUIManager` component
3. Drag panels:
   - Perk Station Panel ← `PerkStationPanel`
   - Pack A Punch Panel ← `PackAPunchPanel`
   - Weapon Buy Panel ← `WeaponBuyPanel`
   - Mystery Chest Panel ← `MysteryChestPanel`

### Step 2: Wire Each UI Controller
For each panel (e.g., PerkStationPanel):
1. Select the panel
2. Find the UI controller component (e.g., PerkStationUI)
3. Drag UI elements:
   - Title Text ← `TitleText`
   - Weapon/Perk Name Text ← `NameText`
   - Cost Text ← `CostText`
   - Description Text ← `DescriptionText`
   - Player Points Text ← `PlayerPointsText`
   - Button ← `BuyButton` (or UpgradeButton/OpenButton)
   - Status Text ← `StatusText`
   - (Plus any panel-specific fields)

### Step 3: Wire Each Interactable
For each interactable (e.g., PerkStation):
1. Select the interactable GameObject
2. Find the interactable component
3. Drag UI controller:
   - UI Controller ← `PerkStationPanel` (then find PerkStationUI component)

---

## Color Reference (Copy-Paste Ready)

### Perk Station Theme
```
Background: rgba(20, 10, 30, 220)   or  #140A1EDC
Title:      #FFD700
Button:     rgba(100, 50, 150, 200) or  #6432C896
```

### Pack-a-Punch Theme
```
Background: rgba(40, 20, 10, 220)   or  #28140ADC
Title:      #FF8C00
Button:     rgba(150, 75, 25, 200)  or  #964B19C8
```

### Weapon Buy Theme
```
Background: rgba(10, 30, 20, 220)   or  #0A1E14DC
Title:      #00FF00
Button:     rgba(25, 100, 50, 200)  or  #196432C8
```

### Mystery Chest Theme
```
Background: rgba(40, 40, 10, 220)   or  #28280ADC
Title:      #FFD700
Button:     rgba(150, 150, 50, 200) or  #969632C8
```

### Status Colors
```
Success/Available:  #00FF00 (green)
Warning/Used:       #FFFF00 (yellow)
Error/No Points:    #FF0000 (red)
Disabled:           #808080 (gray)
```

---

## TextMeshPro Font Settings

### Title Text
```
Font Size: 48
Font Style: Bold
Color: [Theme Accent]
Alignment: Horizontal=Center, Vertical=Top
Enable Auto-Sizing: ☐
```

### Main Text (Weapon/Perk Name)
```
Font Size: 36
Font Style: Bold
Color: #FFFFFF
Alignment: Horizontal=Center, Vertical=Middle
```

### Cost Text
```
Font Size: 28
Font Style: Bold
Color: #FFD700
Alignment: Horizontal=Center, Vertical=Middle
```

### Description Text
```
Font Size: 20
Font Style: Regular
Color: #CCCCCC
Alignment: Horizontal=Center, Vertical=Middle
Wrapping: Enabled
```

### Button Text
```
Font Size: 32
Font Style: Bold
Color: #FFFFFF
Alignment: Horizontal=Center, Vertical=Middle
```

### Status Text
```
Font Size: 22
Font Style: Italic
Color: Dynamic (see Status Colors above)
Alignment: Horizontal=Center, Vertical=Bottom
```

---

## Testing Checklist

### Pre-Play Mode
- [ ] All panels have CanvasGroup (auto-added by manager)
- [ ] All panels start deactivated
- [ ] InteractableUIManager has all 4 panel references
- [ ] Each UI controller has all UI element references
- [ ] Each interactable has UI controller reference
- [ ] Player tagged as "Player"
- [ ] ScoreManager exists in scene

### Play Mode Tests
- [ ] Walk to Perk Station → Panel appears
- [ ] Walk away → Panel disappears
- [ ] Press ESC → Panel closes
- [ ] Button state reflects points
- [ ] Purchase works
- [ ] Panel updates after purchase
- [ ] Test all 4 interactable types
- [ ] Multiple interactables don't conflict

---

## Common Mistakes to Avoid

❌ **Don't:** Forget to tag player as "Player"
✅ **Do:** Set player tag in Inspector

❌ **Don't:** Make trigger collider non-trigger
✅ **Do:** Ensure isTrigger = true (auto-set in OnValidate)

❌ **Don't:** Assign the panel GameObject to UI Controller field
✅ **Do:** Assign the specific UI controller component

❌ **Don't:** Leave UI element references empty
✅ **Do:** Drag all text/button elements to controller

❌ **Don't:** Forget to create loot spawn point for chest
✅ **Do:** Add empty GameObject above chest as spawn point

---

## Quick Start (5 Minutes)

1. **Create Canvas** (2 min)
   - UI > Canvas
   - Add InteractableUIManager
   
2. **Create One Panel** (2 min)
   - UI > Panel under Canvas
   - Add UI controller script
   - Create basic text + button children
   
3. **Link to Interactable** (1 min)
   - Drag UI controller to interactable
   - Test in Play Mode

Then replicate for other 3 types!

---

**Ready to implement! 🚀**
