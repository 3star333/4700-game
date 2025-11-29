# Greybox FPS with Quake/CS Movement

This project includes a basic greybox generator and a first-person controller implementing Quake/CS-style movement: ground friction, air acceleration, and bunnyhop.

## How to Run

1. Open the project in Unity (2022+ should be fine, based on ProjectSettings).
2. Option A: Menu > Tools > Create Greybox FPS Scene (auto-creates `Assets/Scenes/GreyboxScene.unity`).
3. Option B: Create an empty scene. Add a GameObject and attach `GreyboxFPS.GreyboxBootstrap`, then press Play to auto-spawn a floor, walls, light, and Player.

If `Assets/InputSystem_Actions.inputactions` exists and is imported, a `PlayerInput` with the `Player` action map will be used. If not, the controller falls back to local actions.

## Controls

- Move: WASD / Left Stick
- Look: Mouse / Right Stick
- Jump: Space / A (South)
- Sprint: Left Shift / L3
- Crouch: C / B (East)

## Scripts

- `Assets/Scripts/FpsQuakeController.cs` – CharacterController-based Quake movement, mouse look, jump, sprint, crouch.
- `Assets/Scripts/GreyboxBootstrap.cs` – Spawns a simple greybox and a Player at runtime.

## Tuning Parameters (Inspector)

- Move Speeds
  - moveSpeed: base ground speed
  - runMultiplier: sprint factor
  - crouchMultiplier: crouch speed factor
- Quake Physics
  - gravity: downward acceleration
  - jumpSpeed: instantaneous upward speed on jump
  - groundFriction: braking on ground
  - stopSpeed: minimum speed used by friction calc
  - groundAccelerate: acceleration on ground
  - airAccelerate: acceleration while in air
  - airMaxSpeed: clamps wish speed used in air
- Look
  - mouseSensitivity, verticalLookClamp
- Crouch
  - standingHeight, crouchingHeight, crouchLerpSpeed

## Quake Movement Overview

- Friction (ground only):
  - speed = |v_xz|
  - control = max(speed, stopSpeed)
  - drop = control * groundFriction * dt
  - v_xz *= max(speed - drop, 0) / speed
- Accelerate (ground/air):
  - current = dot(v_xz, wishDir)
  - add = wishSpeed - current
  - accelSpeed = min(accel * dt * wishSpeed, add)
  - v_xz += wishDir * accelSpeed
- AirMove clamps `wishSpeed` by `airMaxSpeed`.
- Bunnyhop: as long as you press Jump right as you land, you keep speed (no hard cap other than params).

## Notes

- Uses Unity Input System. Make sure the Input System package is installed and active in Project Settings.
- Controller locks and hides the cursor on play.
- This is a minimal setup for prototyping; expand with your own scene, models, and UI.

Use freely in your project.
