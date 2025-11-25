Weapon scripts and prefabs:
- `BaseWeapon.cs` - Base class for all modular weapons.
- `WeaponController.cs` - Hitscan implementation for bullets/rifle/SMG.
- `WeaponManager.cs` - Holds and manages player's weapons and switching.
- `Mac10.cs` - MAC10 defaults
- `AK101.cs` - AK-101 defaults
- `SKSCarbine.cs` - SKS defaults
- `PumpShotgun.cs` - Shotgun with pellet spread
- `SawedOffShotgun.cs` - Sawed off shotgun with recoil pushback

Instructions:
- Create prefabs for each weapon by making a GameObject, attaching the appropriate weapon script and assigning muzzle/impact/firepoint assets. Use the Inspector to fine tune.
- Add `WeaponManager` on the Player GameObject and set the `weaponMount` transform to where weapons are equipped.
