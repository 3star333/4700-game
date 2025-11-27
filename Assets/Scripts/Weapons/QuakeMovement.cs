using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quake-style movement controller for first-person player movement.
/// Features bunny hopping, air control, and momentum-based physics.
/// Attach to any player GameObject with a CharacterController component.
/// Uses Unity's new Input System.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class QuakeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float groundSpeed = 7f;
    [SerializeField] private float airSpeed = 0.8f; // Reduced for better control
    [SerializeField] private float groundAcceleration = 14f;
    [SerializeField] private float airAcceleration = 10f; // Increased for responsiveness
    [SerializeField] private float maxAirSpeed = 0.6f;
    [SerializeField] private float friction = 6f;

    [Header("Sprint / Slide")]
    [SerializeField] private float sprintMultiplier = 1.6f;
    [SerializeField] private float slideInitialSpeed = 12f;
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private float slideFriction = 1.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.6f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float lookXSensitivity = 2f;
    [SerializeField] private float lookYSensitivity = 2f;

    // Private variables
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float rotationX = 0f;
    private bool jumpPressed = false;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideVelocity = Vector3.zero;
    private float standHeight = 2f;
    private float cameraStandLocalY = 0.6f;
    // External momentum window allows added impulses to influence air control for a short period
    private float externalMomentumTimer = 0f;
    [SerializeField] private float externalMomentumDuration = 0.5f;
    
    // Input System variables
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool wasGroundedLastFrame = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
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
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // initialize heights
        if (controller != null)
        {
            standHeight = Mathf.Max(1f, controller.height);
        }
        if (cameraTransform != null)
        {
            cameraStandLocalY = cameraTransform.localPosition.y;
        }
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
        
        // Reset vertical velocity if grounded and not jumping
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small downward force to keep grounded
        }
        
        // Update external momentum timer
        if (externalMomentumTimer > 0f)
            externalMomentumTimer -= Time.deltaTime;

        // Apply movement (can move in all directions simultaneously)
        if (isGrounded)
        {
            GroundMove(horizontal, vertical);
        }
        else
        {
            AirMove(horizontal, vertical);
            // Apply gravity only in air
            playerVelocity.y -= gravity * Time.deltaTime;
        }
        
        // update slide if active
        UpdateSlide(Time.deltaTime);

        // Smooth crouch / stand transition on controller height and camera
        if (controller != null && cameraTransform != null)
        {
            float targetHeight = isCrouching ? crouchHeight : standHeight;
            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

            // move camera local Y accordingly
            float targetCamY = isCrouching ? cameraStandLocalY * 0.6f : cameraStandLocalY;
            Vector3 camLocal = cameraTransform.localPosition;
            camLocal.y = Mathf.Lerp(camLocal.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
            cameraTransform.localPosition = camLocal;
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
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        // Only jump on button press (not hold)
        if (context.performed && isGrounded)
        {
            // If sliding, carry horizontal slide momentum into the jump
            if (isSliding)
            {
                playerVelocity.x += slideVelocity.x;
                playerVelocity.z += slideVelocity.z;
                // end slide immediately
                isSliding = false;
                slideVelocity = Vector3.zero;
                slideTimer = 0f;
                // allow air momentum window
                externalMomentumTimer = externalMomentumDuration;
            }

            playerVelocity.y = jumpForce;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    // Send Messages fallback
    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Crouch is hold; if performed start crouch, if canceled try to stand
        if (context.performed)
        {
            // If sprinting and moving fast, start slide instead
            if (isSprinting && isGrounded && !isSliding)
            {
                StartSlide();
            }
            else
            {
                isCrouching = true;
            }
        }
        else if (context.canceled)
        {
            // Try to stand up; check for ceiling
            if (CanStand()) isCrouching = false;
        }
    }

    // Send Messages fallback for crouch
    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            if (isSprinting && isGrounded && !isSliding)
            {
                StartSlide();
            }
            else
            {
                isCrouching = true;
            }
        }
        else
        {
            if (CanStand()) isCrouching = false;
        }
    }
    
    // Fallback methods in case Unity is looking for these
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    
    public void OnJump(InputValue value)
    {
        // Only jump on button press (not hold)
        if (value.isPressed && isGrounded)
        {
            playerVelocity.y = jumpForce;
        }
    }

    private void HandleMouseLook()
    {
    float mouseX = lookInput.x * lookXSensitivity;
    float mouseY = lookInput.y * lookYSensitivity * (invertY ? 1f : -1f);
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
        
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    private void GroundMove(float horizontal, float vertical)
    {
        // Don't apply friction to Y velocity
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        
        // Apply friction
        float speed = horizontalVelocity.magnitude;
        if (speed != 0)
        {
            float currentFriction = isSliding ? slideFriction : friction;
            float drop = speed * currentFriction * Time.deltaTime;
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
        
        // Apply sprint and crouch modifiers
        float effectiveSpeed = groundSpeed;
        if (isSprinting) effectiveSpeed *= sprintMultiplier;
        if (isCrouching) effectiveSpeed *= crouchSpeedMultiplier;

        // If sliding, we keep the slide velocity and don't fully use normal acceleration
        if (isSliding)
        {
            // Maintain slide velocity in forward direction
            playerVelocity.x = slideVelocity.x;
            playerVelocity.z = slideVelocity.z;
        }
        else
        {
            // Accelerate (applies to horizontal movement only)
            Accelerate(wishDir, effectiveSpeed, groundAcceleration);
        }
    }

    private void AirMove(float horizontal, float vertical)
    {
        // Preserve current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        
        // Get desired direction - allows diagonal air movement
        Vector3 wishDir = transform.right * horizontal + transform.forward * vertical;
        wishDir.y = 0;
        
    // Only apply air control if player is giving input
    if (wishDir.magnitude > 0.1f)
        {
            wishDir.Normalize();
            
            // Air strafe acceleration - adds velocity in the wish direction
            float currentSpeed = Vector3.Dot(horizontalVelocity, wishDir);
            float addSpeed = airSpeed - currentSpeed;
            
            if (addSpeed > 0)
            {
                // If we recently had external momentum (slide/jump), allow larger air accel
                float momentumBoost = externalMomentumTimer > 0f ? 1.6f : 1f;
                float accelSpeed = airAcceleration * airSpeed * momentumBoost * Time.deltaTime;
                if (accelSpeed > addSpeed)
                    accelSpeed = addSpeed;
                
                playerVelocity.x += wishDir.x * accelSpeed;
                playerVelocity.z += wishDir.z * accelSpeed;
            }
        }
        // If no input, maintain current horizontal velocity (no air friction)
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

    private void StartSlide()
    {
        if (isSliding || !isGrounded) return;
        isSliding = true;
        slideTimer = slideDuration;

        // initial slide velocity in forward direction
        Vector3 forward = transform.forward;
        slideVelocity = new Vector3(forward.x, 0, forward.z).normalized * slideInitialSpeed;

        // add a small downward to keep grounded
        playerVelocity.y = -2f;
    }

    private void UpdateSlide(float dt)
    {
        if (!isSliding) return;

        slideTimer -= dt;
        // gradually reduce slide velocity
        slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, slideFriction * dt);

        // stop slide when timer ends or velocity is small
        if (slideTimer <= 0f || slideVelocity.magnitude < 0.5f)
        {
            isSliding = false;
            slideVelocity = Vector3.zero;
            // after sliding, enter crouch briefly
            isCrouching = true;
        }

        // apply slide velocity to playerVelocity (preserve Y)
        playerVelocity.x = slideVelocity.x;
        playerVelocity.z = slideVelocity.z;
    }

    /// <summary>
    /// Return true if there's room above the player to stand up.
    /// </summary>
    private bool CanStand()
    {
        if (controller == null) return true;
        float checkRadius = controller.radius * 0.9f;
        Vector3 start = transform.position + Vector3.up * (controller.height * 0.5f);
        Vector3 end = transform.position + Vector3.up * (standHeight - checkRadius);
        return !Physics.CheckCapsule(start, end, checkRadius, ~0, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// Apply an impulse to the player's current velocity, used for recoil effects / movement tech.
    /// </summary>
    public void AddImpulse(Vector3 impulse)
    {
    playerVelocity += impulse;
    // open a small window where air control is more permissive to encourage movement tech
    externalMomentumTimer = externalMomentumDuration;
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
