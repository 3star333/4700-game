using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponBuyStation : MonoBehaviour, IInteractable
{
    [Header("Weapon Settings")]
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private int cost = 500;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private bool purchased = false;

    [Header("UI Reference")]
    [SerializeField] private WeaponBuyUI uiController;

    [Header("Trigger Settings")]
    [SerializeField] private bool useTriggerZone = true;
    [SerializeField] private float triggerRadius = 3f;

    public bool Interact(GameObject interactor)
    {
        if (purchased)
        {
            Debug.Log("Weapon already purchased.");
            return false;
        }

        if (ScoreManager.Instance != null && ScoreManager.Instance.SpendPoints(cost))
        {
            purchased = true;
            Debug.Log($"Purchased {weaponName} for {cost} points.");

            // If a weapon prefab has a Weapon component, instantiate and parent it to the player
            if (weaponPrefab != null && interactor != null)
            {
                WeaponManager wm = interactor.GetComponent<WeaponManager>();
                if (wm != null)
                {
                    wm.AddWeapon(weaponPrefab);
                }
                else
                {
                    GameObject newWeapon = Instantiate(weaponPrefab, interactor.transform);
                    newWeapon.transform.localPosition = Vector3.zero;
                    newWeapon.transform.localRotation = Quaternion.identity;
                }
            }
            return true;
        }

        Debug.Log("Not enough points to purchase weapon");
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
            uiController.SetWeaponBuyStation(this, player);
            InteractableUIManager.Instance.ShowWeaponBuyPanel();
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
            Gizmos.color = purchased ? Color.gray : new Color(0f, 1f, 0f, 0.3f);
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
