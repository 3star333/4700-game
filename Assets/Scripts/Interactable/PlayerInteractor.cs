using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerInteractor sends interaction raycasts forward from the player camera
/// and calls Interact on hit objects that implement IInteractable.
/// Attach this to the player GameObject (same object as PlayerInput & QuakeMovement).
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private LayerMask interactLayers = ~0; // Default: everything

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteractor couldn't find a Main Camera in the scene.");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AttemptInteract();
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;
        AttemptInteract();
    }

    private void AttemptInteract()
    {
        if (playerCamera == null) return;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayers))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log($"Interacting with: {hit.collider.name}");
                bool success = interactable.Interact(gameObject);
                // We can show feedback based on success/failure (like sound or hint)
            }
        }
    }
}
