using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quake-style movement controller for first-person player movement.
/// 
/// FEATURES:
/// - NO SPRINTING: Base movement speed only
/// - MELEE SPEED BONUS: 15% faster movement with melee weapons equipped
/// - MOMENTUM-BASED SLIDING: Slide builds momentum over time, capped at maxSlideSpeed
/// - SLIDE JUMPING: Jump from slide for extra momentum boost (slideJumpBoostMultiplier)
/// - AIR STRAFING: 
///   * Keyboard AD strafing: Minimal momentum gain (prevents easy speed building)
///   * Camera-based strafing: High momentum gain when turning camera while strafing (rewards skill)
/// - BUNNY HOPPING: Preserve momentum between jumps for high speeds
/// 
/// Attach to any player GameObject with a CharacterController component.
/// Uses Unity's new Input System.
/// Call UpdateWeaponType() when switching weapons to update speed modifier.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class QuakeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseGroundSpeed = 7f;
    [SerializeField] private float meleeWeaponSpeedBonus = 0.15f; // 15% speed boost for melee weapons
    [SerializeField] private float groundAcceleration = 14f;
    [SerializeField] private float friction = 6f;

    [Header("Air Control Settings")]
    [SerializeField] private float airAcceleration = 2f; // Low to prevent AD strafe momentum
    [SerializeField] private float airStrafeAcceleration = 0.5f; // Minimal for keyboard strafing
    [SerializeField] private float airCameraAcceleration = 12f; // High for camera-based air strafe
    [SerializeField] private float maxAirSpeed = 20f; // Allow high speeds from bunny hopping
    [SerializeField] private float airControlThreshold = 0.1f; // Minimum input for air control

    [Header("Slide Settings")]
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float slideSpeedThreshold = 5f; // Can slide at this speed
    [SerializeField] private float slideInitialBoost = 3f; // Boost when starting slide
    [SerializeField] private float slideMomentumMultiplier = 1.2f; // Momentum builds during slide
    [SerializeField] private float maxSlideSpeed = 18f; // Maximum speed during slide
    [SerializeField] private float slideFriction = 2f; // Low friction to maintain momentum
    [SerializeField] private float slideJumpBoostMultiplier = 1.3f; // Extra boost when jumping from slide
    [SerializeField] private bool requireHeadroomToStand = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 10f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private Transform cameraTransform;

    // Private variables
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float rotationX = 0f;
    private bool jumpPressed = false;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isSliding = false;
    private bool crouchHeld = false;
    private float slideTimer = 0f;
    private Vector3 slideDirection = Vector3.zero;
    private float currentSlideSpeed = 0f;
    private bool hasMeleeWeapon = false; // Track if player has melee weapon equipped
    
    // Input System variables
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 lastLookDirection = Vector3.forward;
    private float cameraRotationDelta = 0f;
    private bool jumpInput;
    private bool wasGroundedLastFrame = false;
    private float defaultControllerHeight;
    private Vector3 defaultControllerCenter;
    private float controllerBottomOffset;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        defaultControllerHeight = controller.height;
        defaultControllerCenter = controller.center;
        controllerBottomOffset = defaultControllerCenter.y - (defaultControllerHeight * 0.5f);
        if (crouchHeight <= 0f)
            crouchHeight = Mathf.Max(defaultControllerHeight * 0.6f, 0.5f);
        
        // If no camera is assigned, try to find the main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform != null && cameraTransform.parent != transform)
            {
                cameraTransform.SetParent(transform);
                cameraTransform.localPosition = new Vector3(0, 0.6f, 0);
            }
        }
        
        // Initialize last look direction
        if (cameraTransform != null)
        {
            lastLookDirection = cameraTransform.forward;
        }
        
        // Check for melee weapon at start
        UpdateWeaponType();
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Handle mouse look
        HandleMouseLook();
        
        // Get input from Input System
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isSliding && !isGrounded)
        {
            StopSlide();
        }
        
        // Reset vertical velocity if grounded and not jumping
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small downward force to keep grounded
        }
        
        // Apply movement (can move in all directions simultaneously)
        if (isSliding)
        {
            SlideMove();
        }
        else if (isGrounded)
        {
            GroundMove(horizontal, vertical);
        }
        else
        {
            AirMove(horizontal, vertical);
            // Apply gravity only in air
            playerVelocity.y -= gravity * Time.deltaTime;
        }
        
        // Move the controller
        controller.Move(playerVelocity * Time.deltaTime);
        
        // Update grounded state for next frame
        wasGroundedLastFrame = isGrounded;
    }
    
    // Input System callbacks - these must be public for Send Messages to work
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        // Only jump on button press (not hold)
        if (context.performed && isGrounded)
        {
            // If sliding, apply extra boost
            if (isSliding)
            {
                float horizontalSpeed = GetHorizontalSpeed();
                playerVelocity.y = jumpForce;
                
                // Apply slide jump boost
                Vector3 horizontalVel = new Vector3(playerVelocity.x, 0, playerVelocity.z);
                if (horizontalVel.magnitude > 0.1f)
                {
                    horizontalVel = horizontalVel.normalized * horizontalSpeed * slideJumpBoostMultiplier;
                    playerVelocity.x = horizontalVel.x;
                    playerVelocity.z = horizontalVel.z;
                }
                
                StopSlide();
            }
            else
            {
                playerVelocity.y = jumpForce;
            }
        }
    }
    
    public void OnJump(InputValue value)
    {
        // Only jump on button press (not hold)
        if (value.isPressed && isGrounded)
        {
            // If sliding, apply extra boost
            if (isSliding)
            {
                float horizontalSpeed = GetHorizontalSpeed();
                playerVelocity.y = jumpForce;
                
                // Apply slide jump boost
                Vector3 horizontalVel = new Vector3(playerVelocity.x, 0, playerVelocity.z);
                if (horizontalVel.magnitude > 0.1f)
                {
                    horizontalVel = horizontalVel.normalized * horizontalSpeed * slideJumpBoostMultiplier;
                    playerVelocity.x = horizontalVel.x;
                    playerVelocity.z = horizontalVel.z;
                }
                
                StopSlide();
            }
            else
            {
                playerVelocity.y = jumpForce;
            }
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        // Calculate camera rotation delta for air strafing
        cameraRotationDelta = Mathf.Abs(mouseX);
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
        
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            
            // Update look direction for air control
            Vector3 currentLookDirection = cameraTransform.forward;
            currentLookDirection.y = 0;
            currentLookDirection.Normalize();
            lastLookDirection = currentLookDirection;
        }
    }

    public float GetMouseSensitivity() => mouseSensitivity;
    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = Mathf.Clamp(value, minSensitivity, maxSensitivity);
    }

    private void GroundMove(float horizontal, float vertical)
    {
        // Don't apply friction to Y velocity
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        
        // Apply friction
        float speed = horizontalVelocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * friction * Time.deltaTime;
            horizontalVelocity *= Mathf.Max(speed - drop, 0) / speed;
            playerVelocity.x = horizontalVelocity.x;
            playerVelocity.z = horizontalVelocity.z;
        }
        
        // Get desired direction - allows diagonal movement (W+A, W+D, etc.)
        Vector3 wishDir = transform.right * horizontal + transform.forward * vertical;
        wishDir.y = 0;
        
        // Normalize only if there's input to preserve magnitude for diagonal movement
        if (wishDir.magnitude > 1f)
        {
            wishDir.Normalize();
        }
        
        // Accelerate (applies to horizontal movement only)
        Accelerate(wishDir, GetGroundSpeed(), groundAcceleration);
    }

    private void AirMove(float horizontal, float vertical)
    {
        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        
        // Get keyboard input direction
        Vector3 wishDir = transform.right * horizontal + transform.forward * vertical;
        wishDir.y = 0;
        
        // Only apply air control if player is giving input
        if (wishDir.magnitude > airControlThreshold)
        {
            wishDir.Normalize();
            
            // Calculate angle between current velocity and wish direction
            float velocityAngle = 0f;
            if (horizontalVelocity.magnitude > 0.1f)
            {
                velocityAngle = Vector3.Angle(horizontalVelocity.normalized, wishDir);
            }
            
            // AD strafing (keyboard only) - minimal momentum gain
            float currentSpeed = Vector3.Dot(horizontalVelocity, wishDir);
            float addSpeed = airStrafeAcceleration - currentSpeed;
            
            if (addSpeed > 0)
            {
                float accelSpeed = airAcceleration * Time.deltaTime;
                if (accelSpeed > addSpeed)
                    accelSpeed = addSpeed;
                
                playerVelocity.x += wishDir.x * accelSpeed;
                playerVelocity.z += wishDir.z * accelSpeed;
            }
            
            // Camera-based air strafing - builds momentum when turning camera
            // This rewards skillful air control by turning into the strafe direction
            if (cameraRotationDelta > 0.1f && velocityAngle < 90f)
            {
                // Apply camera-based acceleration (Quake-style strafe jumping)
                float cameraBonus = cameraRotationDelta * airCameraAcceleration * Time.deltaTime;
                
                // Apply in the wish direction
                playerVelocity.x += wishDir.x * cameraBonus;
                playerVelocity.z += wishDir.z * cameraBonus;
            }
        }

        // Clamp to max air speed
        if (maxAirSpeed > 0f)
        {
            Vector3 clampedHorizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
            float speed = clampedHorizontalVelocity.magnitude;
            if (speed > maxAirSpeed)
            {
                clampedHorizontalVelocity = clampedHorizontalVelocity.normalized * maxAirSpeed;
                playerVelocity.x = clampedHorizontalVelocity.x;
                playerVelocity.z = clampedHorizontalVelocity.z;
            }
        }
    }

    private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        // Only accelerate horizontally
        Vector3 currentVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        float currentSpeed = Vector3.Dot(currentVelocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        
        if (addSpeed <= 0)
            return;
            
        float accelSpeed = accel * wishSpeed * Time.deltaTime;
        
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;
            
        playerVelocity.x += wishDir.x * accelSpeed;
        playerVelocity.z += wishDir.z * accelSpeed;
    }

    /// <summary>
    /// Apply an impulse to the player's current velocity, used for recoil effects / movement tech.
    /// </summary>
    public void AddImpulse(Vector3 impulse)
    {
        playerVelocity += impulse;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouchHeld = true;
            BeginCrouchOrSlide();
        }
        else if (context.canceled)
        {
            crouchHeld = false;
            EndCrouch();
        }
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            crouchHeld = true;
            BeginCrouchOrSlide();
        }
        else
        {
            crouchHeld = false;
            EndCrouch();
        }
    }

    private void BeginCrouchOrSlide()
    {
        if (isSliding)
            return;

        if (isGrounded && GetHorizontalSpeed() >= slideSpeedThreshold)
        {
            StartSlide();
        }
        else
        {
            StartCrouch();
        }
    }

    private void StartCrouch()
    {
        if (isCrouching)
            return;

        isCrouching = true;
        controller.height = crouchHeight;
        float newCenterY = controllerBottomOffset + crouchHeight * 0.5f;
        controller.center = new Vector3(defaultControllerCenter.x, newCenterY, defaultControllerCenter.z);
    }

    private void StopCrouch()
    {
        if (!isCrouching || isSliding)
            return;

        if (requireHeadroomToStand && !CanStand())
            return;

        isCrouching = false;
        controller.height = defaultControllerHeight;
        controller.center = defaultControllerCenter;
    }

    private void StartSlide()
    {
        StartCrouch();
        isSliding = true;
        
        // Calculate slide direction based on current velocity or forward direction
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        if (horizontalVelocity.magnitude < 0.1f)
        {
            slideDirection = transform.forward;
            currentSlideSpeed = baseGroundSpeed;
        }
        else
        {
            slideDirection = horizontalVelocity.normalized;
            currentSlideSpeed = horizontalVelocity.magnitude;
        }
        
        // Apply initial slide boost
        currentSlideSpeed += slideInitialBoost;
        
        // Apply boosted velocity
        playerVelocity.x = slideDirection.x * currentSlideSpeed;
        playerVelocity.z = slideDirection.z * currentSlideSpeed;
    }

    private void SlideMove()
    {
        // Build momentum during slide
        currentSlideSpeed += currentSlideSpeed * (slideMomentumMultiplier - 1f) * Time.deltaTime;
        
        // Clamp to max slide speed
        currentSlideSpeed = Mathf.Min(currentSlideSpeed, maxSlideSpeed);
        
        // Apply friction
        float friction = slideFriction * Time.deltaTime;
        currentSlideSpeed = Mathf.Max(currentSlideSpeed - friction, 0f);
        
        // Allow slight steering during slide (limited air control)
        Vector3 wishDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (wishDir.sqrMagnitude > 0.01f)
        {
            wishDir.Normalize();
            
            // Blend slide direction towards input (limited steering)
            slideDirection = Vector3.Lerp(slideDirection, wishDir, Time.deltaTime * 1.5f);
            slideDirection.Normalize();
        }
        
        // Apply slide velocity
        playerVelocity.x = slideDirection.x * currentSlideSpeed;
        playerVelocity.z = slideDirection.z * currentSlideSpeed;
        playerVelocity.y = -2f; // Keep grounded
        
        // Stop slide if speed is too low or not grounded
        if (currentSlideSpeed < slideSpeedThreshold * 0.5f || !isGrounded)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        if (!isSliding)
            return;

        isSliding = false;
        slideTimer = 0f;
        currentSlideSpeed = 0f;
        
        if (!crouchHeld)
        {
            StopCrouch();
        }
    }

    private void EndCrouch()
    {
        if (isSliding)
        {
            StopSlide();
        }
        StopCrouch();
    }

    private bool CanStand()
    {
        float radius = controller.radius * 0.95f;
        Bounds bounds = controller.bounds;
        Vector3 bottom = new Vector3(bounds.center.x, bounds.min.y + radius, bounds.center.z);
        Vector3 top = bottom + Vector3.up * (defaultControllerHeight - radius * 2f);
        return !Physics.CheckCapsule(bottom, top, radius, Physics.AllLayers, QueryTriggerInteraction.Ignore);
    }

    private float GetHorizontalSpeed()
    {
        Vector3 horizontal = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        return horizontal.magnitude;
    }

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
    
    /// <summary>
    /// Call this to update movement speed when weapon changes.
    /// Checks if current weapon is melee or ranged.
    /// </summary>
    public void UpdateWeaponType()
    {
        // Check for melee weapon component on player or children
        MeleeWeapon meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        hasMeleeWeapon = (meleeWeapon != null);
        
        // Alternative: Check WeaponManager if you have one
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager != null && weaponManager.CurrentWeapon != null)
        {
            hasMeleeWeapon = weaponManager.CurrentWeapon is MeleeWeapon;
        }
    }

    // Debug info
    private void OnGUI()
    {
        if (!Debug.isDebugBuild)
            return;
            
        Vector3 vel = playerVelocity;
        vel.y = 0;
        float speed = vel.magnitude;
        
        GUI.Label(new Rect(10, 10, 400, 20), $"Speed: {speed:F2} units/s");
        GUI.Label(new Rect(10, 30, 400, 20), $"Grounded: {isGrounded}");
        GUI.Label(new Rect(10, 50, 400, 20), $"Velocity: {playerVelocity}");
    }
}