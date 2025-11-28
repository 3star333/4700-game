# Interactable GUI System - Setup Guide

## Project: Gothic Wave-Survival Shooter (COD Zombies Style)

---

## 🎯 Architecture Overview

The GUI system consists of three layers:

1. **InteractableUIManager** (Global Singleton)
   - Manages showing/hiding all UI panels
   - Handles fade in/out animations
   - Processes ESC key to close panels

2. **Individual UI Controllers** (One per interactable type)
   - PerkStationUI.cs
   - PackAPunchUI.cs
   - WeaponBuyUI.cs
   - MysteryChestUI.cs

3. **Updated Interactable Scripts** (Backend logic unchanged)
   - Added trigger zones (OnTriggerEnter/Exit)
   - Added UI controller references
   - Show/hide UI when player enters/exits range

---

## 📦 Unity Setup Instructions

### Step 1: Create the Main UI Canvas

1. In your scene hierarchy, right-click and select **UI > Canvas**
2. Rename it to `InteractableCanvas`
3. Set Canvas properties:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080
     - Match: 0.5 (balance between width and height)

4. Add the `InteractableUIManager` component to the Canvas:
   - Select `InteractableCanvas`
   - Add Component > Scripts > **InteractableUIManager**

---

### Step 2: Create Panel Structure

Create four panels as children of the `InteractableCanvas`. For each panel:

#### 2A. Create Perk Station Panel

1. Right-click `InteractableCanvas` > UI > Panel
2. Rename to `PerkStationPanel`
3. Set RectTransform:
   - Anchor: Center
   - Width: 600, Height: 700
   - Position: X=0, Y=0

4. Set Panel background:
   - Image Color: `rgba(20, 10, 30, 220)` (dark gothic purple)

5. Add UI elements as children:

**Title Text:**
- UI > Text - TextMeshPro
- Name: `TitleText`
- Text: "PERK STATION"
- Font Size: 48
- Color: `#FFD700` (gold)
- Alignment: Center, Top
- Position: Top of panel

**Perk Name Text:**
- UI > Text - TextMeshPro
- Name: `PerkNameText`
- Text: "Double Tap"
- Font Size: 36
- Color: `#FFFFFF`
- Alignment: Center

**Cost Text:**
- UI > Text - TextMeshPro
- Name: `CostText`
- Text: "Cost: 500 Points"
- Font Size: 28
- Color: `#FFD700`

**Description Text:**
- UI > Text - TextMeshPro
- Name: `DescriptionText`
- Text: "[STUB] Perk description..."
- Font Size: 20
- Color: `#CCCCCC`
- Enable Word Wrapping

**Player Points Text:**
- UI > Text - TextMeshPro
- Name: `PlayerPointsText`
- Text: "Your Points: 0"
- Font Size: 24
- Color: `#00FF00`

**Buy Button:**
- UI > Button - TextMeshPro
- Name: `BuyButton`
- Button Color: `rgba(100, 50, 150, 200)` (purple)
- Text: "PURCHASE PERK"
- Font Size: 32

**Status Text:**
- UI > Text - TextMeshPro
- Name: `StatusText`
- Text: "Press E or Click to Buy"
- Font Size: 22
- Color: `#00FF00`

6. Add `PerkStationUI` component to `PerkStationPanel`:
   - Select the panel
   - Add Component > Scripts > **PerkStationUI**

7. Wire up UI references in PerkStationUI:
   - Drag each text/button element to corresponding field

---

#### 2B. Create Pack-a-Punch Panel

Same process as Perk Station with these differences:

1. Panel name: `PackAPunchPanel`
2. Background color: `rgba(40, 20, 10, 220)` (dark orange-brown)
3. Title: "PACK-A-PUNCH"
4. Title color: `#FF8C00` (orange)
5. Additional field: `WeaponNameText` and `MultiplierText`
6. Button text: "UPGRADE WEAPON"
7. Add `PackAPunchUI` component and wire references

---

#### 2C. Create Weapon Buy Panel

1. Panel name: `WeaponBuyPanel`
2. Background color: `rgba(10, 30, 20, 220)` (dark green)
3. Title: "WEAPON BUY STATION"
4. Title color: `#00FF00` (green)
5. Additional element: `WeaponIconImage` (UI > Image)
   - Size: 256x256
   - Preserve Aspect: True
6. Button text: "BUY WEAPON"
7. Add `WeaponBuyUI` component and wire references

---

#### 2D. Create Mystery Chest Panel

1. Panel name: `MysteryChestPanel`
2. Background color: `rgba(40, 40, 10, 220)` (dark yellow)
3. Title: "MYSTERY CHEST"
4. Title color: `#FFD700` (gold)
5. Additional field: `RarityInfoText` (for showing drop rates)
   - Use Rich Text enabled
   - Font Size: 18
6. Button text: "OPEN CHEST"
7. Add `MysteryChestUI` component and wire references

---

### Step 3: Wire InteractableUIManager References

1. Select `InteractableCanvas`
2. In the `InteractableUIManager` component:
   - Drag `PerkStationPanel` to **Perk Station Panel** field
   - Drag `PackAPunchPanel` to **Pack A Punch Panel** field
   - Drag `WeaponBuyPanel` to **Weapon Buy Panel** field
   - Drag `MysteryChestPanel` to **Mystery Chest Panel** field
3. Set **Fade Duration**: 0.2
4. Enable **Enable ESC To Close**: ✓

---

### Step 4: Setup Interactable GameObjects

For each interactable in your scene:

#### 4A. Perk Station Setup

1. Select your Perk Station GameObject
2. Ensure it has the `PerkStation` component
3. In PerkStation component:
   - **Perk Settings**: Configure type, cost, multiplier
   - **UI Reference**:
     - Drag the `PerkStationPanel` from Canvas
     - Find the `PerkStationUI` component on it
   - **Trigger Settings**:
     - Use Trigger Zone: ✓
     - Trigger Radius: 3
4. The script will auto-create a SphereCollider (trigger) in OnValidate

#### 4B. Pack-a-Punch Setup

Same process:
1. Select Pack-a-Punch GameObject
2. In PackAPunch component:
   - UI Reference: Drag `PackAPunchPanel`'s `PackAPunchUI` component
   - Trigger Radius: 3

#### 4C. Weapon Buy Station Setup

1. Select Weapon Buy Station GameObject
2. In WeaponBuyStation component:
   - Weapon Settings: Name, cost, prefab
   - UI Reference: Drag `WeaponBuyPanel`'s `WeaponBuyUI` component
   - Trigger Radius: 3
3. (Optional) Set weapon icon in `WeaponBuyUI`:
   - Select `WeaponBuyPanel`
   - In `WeaponBuyUI` component
   - Drag sprite to **Weapon Icon** field

#### 4D. Mystery Chest Setup

1. Select Mystery Chest GameObject
2. In LootChest component:
   - Configure rarity weights and weapon arrays
   - UI Reference: Drag `MysteryChestPanel`'s `MysteryChestUI` component
   - Trigger Radius: 3

---

### Step 5: Player Tag Setup

Ensure your player GameObject has the tag "Player":

1. Select your player GameObject
2. At the top of Inspector: Tag dropdown
3. Select **Player** (or create it if it doesn't exist)

---

### Step 6: Testing

1. Enter Play Mode
2. Walk up to any interactable
3. Panel should fade in when you enter trigger zone
4. Panel should show:
   - Current item info
   - Your points
   - Purchase button state
5. Click button or press E to interact
6. Walk away or press ESC to close panel

---

## 🎨 Gothic UI Styling Tips

To enhance the dark gothic theme:

### Recommended Colors
- **Perk Station**: Purple/Magenta tones
  - Background: `#140A1E` (dark purple)
  - Accent: `#FF00FF` (magenta)
  
- **Pack-a-Punch**: Orange/Bronze tones
  - Background: `#281410` (dark brown)
  - Accent: `#FF8C00` (orange)
  
- **Weapon Buy**: Green/Military tones
  - Background: `#0A1E14` (dark green)
  - Accent: `#00FF00` (green)
  
- **Mystery Chest**: Gold/Yellow tones
  - Background: `#28280A` (dark yellow)
  - Accent: `#FFD700` (gold)

### Fonts
- Use bold, condensed fonts for titles
- Consider importing free gothic fonts from Google Fonts:
  - "Nosifer"
  - "Creepster"
  - "Metal Mania"

### Effects
- Add drop shadow to text (Component > Shadow)
- Add outline to buttons (Component > Outline)
- Consider particle effects when panels open

---

## 🔧 Advanced Customization

### Adding Sound Effects

In each UI controller (e.g., PerkStationUI.cs), add:

```csharp
[Header("Audio")]
[SerializeField] private AudioClip openSound;
[SerializeField] private AudioClip purchaseSound;
[SerializeField] private AudioClip failSound;
private AudioSource audioSource;

private void Start()
{
    audioSource = gameObject.AddComponent<AudioSource>();
    // ... existing code
}

private void OnEnable()
{
    if (openSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(openSound);
    }
    UpdateUI();
}

public void OnBuyButton()
{
    // ... existing purchase logic
    if (success && purchaseSound != null)
    {
        audioSource.PlayOneShot(purchaseSound);
    }
    else if (!success && failSound != null)
    {
        audioSource.PlayOneShot(failSound);
    }
}
```

### Adding Animations

1. Select a panel (e.g., PerkStationPanel)
2. Add Component > Animator
3. Create Animation Controller:
   - Assets > Create > Animation Controller
   - Name it `PerkStationPanelAnimator`
4. Create animations for:
   - Panel slide in
   - Button pulse
   - Text glow
5. Assign controller to Animator component

### Making Panels 3D/Diegetic

To make panels appear in world space:

1. Change Canvas Render Mode to **World Space**
2. Scale canvas down: 0.01, 0.01, 0.01
3. Position canvas in front of interactable
4. Add to interactable's trigger to rotate panel toward player

---

## 📋 Checklist

- [ ] InteractableCanvas created with InteractableUIManager
- [ ] All 4 panels created with proper UI elements
- [ ] All 4 UI controllers added and wired
- [ ] InteractableUIManager panel references set
- [ ] All interactables have UI controller references
- [ ] Player has "Player" tag
- [ ] Trigger zones properly sized
- [ ] Tested all interactions in Play Mode
- [ ] Gothic styling applied
- [ ] (Optional) Sound effects added
- [ ] (Optional) Animations added

---

## 🐛 Troubleshooting

**Panel doesn't show up:**
- Check InteractableUIManager has panel reference
- Check interactable has UI controller reference
- Ensure trigger collider is present and is trigger
- Check player has "Player" tag or QuakeMovement component

**Button doesn't work:**
- Check button has listener in UI controller Start()
- Verify UI controller is on the panel GameObject
- Check Event System exists in scene (should auto-create with Canvas)

**Points not updating:**
- Ensure ScoreManager exists in scene
- Check ScoreManager.Instance is not null
- Test with debug logs in Update() method

**ESC doesn't close panel:**
- Check "Enable ESC To Close" in InteractableUIManager
- Ensure no other script is consuming ESC input

---

## 📞 Support

If you encounter issues:
1. Check Unity Console for error messages
2. Verify all serialized fields are assigned in Inspector
3. Use Debug.Log() statements in ShowUI/HideUI methods
4. Ensure all scripts compiled without errors

---

**System Complete! Enjoy your gothic interactable GUI system! 🎮**
