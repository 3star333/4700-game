using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private int cost = 100;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private float openRotation = 90f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Collider doorCollider;

    private void Start()
    {
        closedRotation = transform.rotation;
        openedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openRotation, 0));
        doorCollider = GetComponent<Collider>();
    }

    public bool Interact(GameObject interactor)
    {
        if (!isLocked)
        {
            if (!isOpen) OpenDoor();
            return true; // already open/unlocked
        }

        // Attempt to spend points
        if (ScoreManager.Instance != null && ScoreManager.Instance.SpendPoints(cost))
        {
            isLocked = false;
            OpenDoor();
            return true;
        }

        // Not enough points
        Debug.Log("Not enough points to open the door");
        return false;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        // Disable collider to let player walk through
        if (doorCollider != null) doorCollider.enabled = false;
        // Start opening animation (simple rotation)
        StopAllCoroutines();
        StartCoroutine(OpenDoorCoroutine());
    }

    private System.Collections.IEnumerator OpenDoorCoroutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(closedRotation, openedRotation, t);
            yield return null;
        }
    }

    // Editor helper - show cost as name
#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"Door_{(isLocked?"Locked":"Unlocked")}_Cost{cost}";
    }
#endif
}
