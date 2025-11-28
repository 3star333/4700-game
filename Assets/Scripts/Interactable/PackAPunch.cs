using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PackAPunch : MonoBehaviour, IInteractable
{
    [Header("Pack-a-Punch Settings")]
    [SerializeField] private int cost = 1000;
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private bool used = false;

    [Header("UI Reference")]
    [SerializeField] private PackAPunchUI uiController;

    [Header("Trigger Settings")]
    [SerializeField] private bool useTriggerZone = true;
    [SerializeField] private float triggerRadius = 3f;

    public bool Interact(GameObject interactor)
    {
        if (used)
        {
            Debug.Log("Pack-A-Punch already used.");
            return false;
        }

        if (ScoreManager.Instance == null)
            return false;

        if (!ScoreManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points for Pack-A-Punch.");
            return false;
        }

        // Find the player's WeaponController and apply upgrade
        // Try WeaponManager first
        WeaponManager wm = interactor.GetComponent<WeaponManager>();
        if (wm != null)
        {
            wm.ApplyPackAPunchToCurrent(damageMultiplier);
            used = true;
            Debug.Log("Pack-A-Punch applied to player's current weapon by WeaponManager.");
            return true;
        }

        Weapon bw = interactor.GetComponentInChildren<Weapon>();
        if (bw != null)
        {
            bw.ApplyPackAPunch(damageMultiplier);
            used = true;
            Debug.Log("Pack-A-Punch applied to player's weapon.");
            return true;
        }

        Debug.LogWarning("Player has no WeaponController to upgrade.");
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useTriggerZone) return;

        // Check if the player entered the trigger
        if (other.CompareTag("Player") || other.GetComponent<QuakeMovement>() != null)
        {
            ShowUI(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!useTriggerZone) return;

        // Check if the player left the trigger
        if (other.CompareTag("Player") || other.GetComponent<QuakeMovement>() != null)
        {
            HideUI();
        }
    }

    private void ShowUI(GameObject player)
    {
        if (uiController != null && InteractableUIManager.Instance != null)
        {
            uiController.SetPackAPunch(this, player);
            InteractableUIManager.Instance.ShowPackAPunchPanel();
        }
    }

    private void HideUI()
    {
        if (InteractableUIManager.Instance != null)
        {
            InteractableUIManager.Instance.HideCurrentPanel();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (useTriggerZone)
        {
            Gizmos.color = used ? Color.gray : new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }

    private void OnValidate()
    {
        // Auto-setup trigger collider in editor
        if (useTriggerZone)
        {
            SphereCollider trigger = GetComponent<SphereCollider>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<SphereCollider>();
            }
            trigger.isTrigger = true;
            trigger.radius = triggerRadius;
        }
    }
}
