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

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private Transform cameraTransform;

    // Private variables
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float rotationX = 0f;
    private bool jumpPressed = false;
    private bool isGrounded = false;
    
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
        Accelerate(wishDir, groundSpeed, groundAcceleration);
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
                float accelSpeed = airAcceleration * airSpeed * Time.deltaTime;
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

    /// <summary>
    /// Apply an impulse to the player's current velocity, used for recoil effects / movement tech.
    /// </summary>
    public void AddImpulse(Vector3 impulse)
    {
        playerVelocity += impulse;
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
