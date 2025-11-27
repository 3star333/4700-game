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

    [Header("Mid-Air Momentum Settings")]
    [SerializeField] private float airMomentumMultiplier = 1.4f;
    [SerializeField] private float maxAirMomentumSpeed = 14f;

    [Header("Crouch / Slide Settings")]
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float crouchSpeedMultiplier = 0.6f;
    [SerializeField] private float slideSpeedThreshold = 6f;
    [SerializeField] private float slideImpulse = 8f;
    [SerializeField] private float slideDuration = 0.75f;
    [SerializeField] private float slideFriction = 10f;
    [SerializeField] private bool requireHeadroomToStand = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Mouse Look Settings")]
    [Header("Look Sensitivity")]
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
    
    // Input System variables
    private Vector2 moveInput;
    private Vector2 lookInput;
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
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        // Only jump on button press (not hold)
        if (context.performed && isGrounded)
        {
            playerVelocity.y = jumpForce;
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
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
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
                float accelSpeed = airAcceleration * airSpeed * airMomentumMultiplier * Time.deltaTime;
                if (accelSpeed > addSpeed)
                    accelSpeed = addSpeed;
                
                playerVelocity.x += wishDir.x * accelSpeed;
                playerVelocity.z += wishDir.z * accelSpeed;
            }
        }

        if (maxAirMomentumSpeed > 0f)
        {
            Vector3 clampedHorizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
            float speed = clampedHorizontalVelocity.magnitude;
            if (speed > maxAirMomentumSpeed)
            {
                clampedHorizontalVelocity = clampedHorizontalVelocity.normalized * maxAirMomentumSpeed;
                playerVelocity.x = clampedHorizontalVelocity.x;
                playerVelocity.z = clampedHorizontalVelocity.z;
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
        slideTimer = slideDuration;
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        if (horizontalVelocity.magnitude < 0.1f)
            horizontalVelocity = transform.forward;
        playerVelocity += horizontalVelocity.normalized * slideImpulse;
    }

    private void SlideMove()
    {
        slideTimer -= Time.deltaTime;

        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        float speed = horizontalVelocity.magnitude;
        if (speed > 0f)
        {
            float drop = slideFriction * Time.deltaTime;
            speed = Mathf.Max(speed - drop, 0f);
            horizontalVelocity = horizontalVelocity.normalized * speed;
            playerVelocity.x = horizontalVelocity.x;
            playerVelocity.z = horizontalVelocity.z;
        }

        Vector3 wishDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (wishDir.sqrMagnitude > 0.1f)
        {
            wishDir.Normalize();
            playerVelocity.x += wishDir.x * groundAcceleration * 0.5f * Time.deltaTime;
            playerVelocity.z += wishDir.z * groundAcceleration * 0.5f * Time.deltaTime;
        }

        playerVelocity.y = -2f;

        if (slideTimer <= 0f || !isGrounded)
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
        if (isCrouching && !isSliding)
            return groundSpeed * crouchSpeedMultiplier;
        return groundSpeed;
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
