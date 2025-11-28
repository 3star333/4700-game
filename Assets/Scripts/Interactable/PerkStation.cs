using UnityEngine;

public enum PerkType
{
    DoubleDamage,
    SpeedBoost,
    ExtraHealth
}

[RequireComponent(typeof(Collider))]
public class PerkStation : MonoBehaviour, IInteractable
{
    [Header("Perk Settings")]
    [SerializeField] private PerkType type = PerkType.DoubleDamage;
    [SerializeField] private int cost = 500;
    [SerializeField] private float multiplier = 1.5f; // Effect, meaning depends on perk type
    [SerializeField] private bool purchased = false;

    [Header("UI Reference")]
    [SerializeField] private PerkStationUI uiController;

    [Header("Trigger Settings")]
    [SerializeField] private bool useTriggerZone = true;
    [SerializeField] private float triggerRadius = 3f;

    public bool Interact(GameObject interactor)
    {
        if (purchased)
            return false;

        if (ScoreManager.Instance == null)
            return false;

        if (!ScoreManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points to buy perk.");
            return false;
        }

        // Apply perk
        switch (type)
        {
            case PerkType.DoubleDamage:
            {
                Weapon bw = interactor.GetComponentInChildren<Weapon>();
                if (bw != null)
                {
                    bw.ModifyDamage(multiplier);
                    purchased = true;
                    Debug.Log("Perk: Double Damage applied");
                    return true;
                }
                break;
            }
            case PerkType.SpeedBoost:
            {
                QuakeMovement qm = interactor.GetComponent<QuakeMovement>();
                if (qm != null)
                {
                    qm.GetType(); // placeholder; in the future, give an API to add speed buff
                    Debug.Log("Speed Boost perk purchased (implementation pending)");
                    purchased = true;
                    return true;
                }
                break;
            }
            case PerkType.ExtraHealth:
            {
                // Implement player health script logic to add max health
                Debug.Log("Extra health perk purchased (not yet implemented)");
                purchased = true;
                return true;
            }
        }

        // If we couldn't apply the perk
        Debug.LogWarning("Failed to apply perk: player missing component");
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
            uiController.SetPerkStation(this, player);
            InteractableUIManager.Instance.ShowPerkStationPanel();
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
            Gizmos.color = purchased ? Color.gray : new Color(1f, 0f, 1f, 0.3f);
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
