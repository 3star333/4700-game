using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LootChest : MonoBehaviour, IInteractable
{
    [Header("Spawn Settings")]
    public Transform lootSpawnPoint;
    public bool openOnlyOnce = true;
    private bool hasBeenOpened = false;

    [Header("Weapon Prefabs By Rarity")]
    public GameObject[] commonWeapons;
    public GameObject[] uncommonWeapons;
    public GameObject[] rareWeapons;
    public GameObject[] legendaryWeapons;

    [Header("Rarity Weights")]
    [Tooltip("Relative chance - higher numbers mean more common")]
    public float legendaryWeight = 7f;
    public float rareWeight = 20f;
    public float uncommonWeight = 40f;
    public float commonWeight = 60f;

    [Header("Optional Cost")]
    public int cost = 0;  // 0 = free to open

    [Header("UI Reference")]
    [SerializeField] private MysteryChestUI uiController;

    [Header("Trigger Settings")]
    [SerializeField] private bool useTriggerZone = true;
    [SerializeField] private float triggerRadius = 3f;

    public bool Interact(GameObject interactor)
    {
        if (openOnlyOnce && hasBeenOpened)
        {
            Debug.Log("Chest already opened.");
            return false;
        }

        // Check if player has enough points (if cost > 0)
        if (cost > 0)
        {
            if (ScoreManager.Instance == null || !ScoreManager.Instance.SpendPoints(cost))
            {
                Debug.Log($"Not enough points to open chest (costs {cost})");
                return false;
            }
        }

        OpenChest();
        return true;
    }

    private void OpenChest()
    {
        hasBeenOpened = true;

        Rarity chosenRarity = RollRarity();
        GameObject weaponPrefab = PickWeaponForRarity(chosenRarity);

        if (weaponPrefab != null && lootSpawnPoint != null)
        {
            Vector3 spawnPos = lootSpawnPoint.position + Vector3.up * 0.5f;
            GameObject spawnedWeapon = Instantiate(weaponPrefab, spawnPos, lootSpawnPoint.rotation);
            
            // Add a little upward force for visual pop
            Rigidbody rb = spawnedWeapon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            }
            
            Debug.Log($"Chest opened! Spawned {chosenRarity} weapon.");
        }
        else
        {
            Debug.LogWarning("LootChest: Missing weapon prefab or spawn point");
        }

        // TODO: Add opening animation, particle effect, sound
    }

    private enum Rarity { Common, Uncommon, Rare, Legendary }

    private Rarity RollRarity()
    {
        float totalWeight = legendaryWeight + rareWeight + uncommonWeight + commonWeight;
        float roll = Random.value * totalWeight;

        if (roll < legendaryWeight)
            return Rarity.Legendary;

        roll -= legendaryWeight;
        if (roll < rareWeight)
            return Rarity.Rare;

        roll -= rareWeight;
        if (roll < uncommonWeight)
            return Rarity.Uncommon;

        return Rarity.Common;
    }

    private GameObject PickWeaponForRarity(Rarity rarity)
    {
        GameObject[] pool = null;

        switch (rarity)
        {
            case Rarity.Legendary: pool = legendaryWeapons; break;
            case Rarity.Rare: pool = rareWeapons; break;
            case Rarity.Uncommon: pool = uncommonWeapons; break;
            case Rarity.Common: pool = commonWeapons; break;
        }

        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning($"LootChest: No weapons for {rarity} rarity");
            return null;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    private void OnValidate()
    {
        // Validation in editor
        if (lootSpawnPoint == null)
        {
            Debug.LogWarning($"LootChest '{gameObject.name}' missing spawn point!");
        }

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

    private void OnDrawGizmosSelected()
    {
        if (lootSpawnPoint != null)
        {
            Gizmos.color = hasBeenOpened ? Color.gray : Color.yellow;
            Gizmos.DrawWireSphere(lootSpawnPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, lootSpawnPoint.position);
        }

        if (useTriggerZone)
        {
            Gizmos.color = hasBeenOpened ? Color.gray : new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
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
            uiController.SetLootChest(this, player);
            InteractableUIManager.Instance.ShowMysteryChestPanel();
        }
    }

    private void HideUI()
    {
        if (InteractableUIManager.Instance != null)
        {
            InteractableUIManager.Instance.HideCurrentPanel();
        }
    }
}
