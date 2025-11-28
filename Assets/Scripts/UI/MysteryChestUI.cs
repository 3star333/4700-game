using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Controller for Mystery Chest (Loot Chest) panel.
/// Displays chest information and handles open button.
/// </summary>
public class MysteryChestUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI playerPointsText;
    [SerializeField] private Button openButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI rarityInfoText;

    [Header("Mystery Chest Reference")]
    [SerializeField] private LootChest lootChest;

    private GameObject player;

    private void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(OnOpenButton);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void Update()
    {
        // Update player points display in real-time
        if (playerPointsText != null && ScoreManager.Instance != null)
        {
            playerPointsText.text = $"Your Points: {ScoreManager.Instance.Points}";
        }
    }

    public void SetLootChest(LootChest chest, GameObject playerObject)
    {
        lootChest = chest;
        player = playerObject;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (lootChest == null) return;

        // Get loot chest info using reflection
        var costField = lootChest.GetType().GetField("cost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var hasBeenOpenedField = lootChest.GetType().GetField("hasBeenOpened", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var openOnlyOnceField = lootChest.GetType().GetField("openOnlyOnce", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        // Get rarity weights
        var legendaryWeightField = lootChest.GetType().GetField("legendaryWeight", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var rareWeightField = lootChest.GetType().GetField("rareWeight", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var uncommonWeightField = lootChest.GetType().GetField("uncommonWeight", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var commonWeightField = lootChest.GetType().GetField("commonWeight", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        int cost = (int)costField.GetValue(lootChest);
        bool hasBeenOpened = (bool)hasBeenOpenedField.GetValue(lootChest);
        bool openOnlyOnce = (bool)openOnlyOnceField.GetValue(lootChest);

        float legendaryWeight = (float)legendaryWeightField.GetValue(lootChest);
        float rareWeight = (float)rareWeightField.GetValue(lootChest);
        float uncommonWeight = (float)uncommonWeightField.GetValue(lootChest);
        float commonWeight = (float)commonWeightField.GetValue(lootChest);

        // Update title
        if (titleText != null)
        {
            titleText.text = "MYSTERY CHEST";
        }

        // Update cost
        if (costText != null)
        {
            if (cost > 0)
            {
                costText.text = $"Cost: {cost} Points";
            }
            else
            {
                costText.text = "FREE";
            }
        }

        // Update description
        if (descriptionText != null)
        {
            descriptionText.text = "Spin the Mystery Box for a random weapon!\nGet lucky and win a powerful weapon.";
        }

        // Update rarity info
        if (rarityInfoText != null)
        {
            float total = legendaryWeight + rareWeight + uncommonWeight + commonWeight;
            float legendaryChance = (legendaryWeight / total) * 100f;
            float rareChance = (rareWeight / total) * 100f;
            float uncommonChance = (uncommonWeight / total) * 100f;
            float commonChance = (commonWeight / total) * 100f;

            rarityInfoText.text = $"<color=#FFD700>Legendary: {legendaryChance:F1}%</color>\n" +
                                 $"<color=#9B30FF>Rare: {rareChance:F1}%</color>\n" +
                                 $"<color=#4169E1>Uncommon: {uncommonChance:F1}%</color>\n" +
                                 $"<color=#FFFFFF>Common: {commonChance:F1}%</color>";
        }

        // Update player points
        if (playerPointsText != null && ScoreManager.Instance != null)
        {
            playerPointsText.text = $"Your Points: {ScoreManager.Instance.Points}";
        }

        // Update button state
        if (openButton != null)
        {
            if (openOnlyOnce && hasBeenOpened)
            {
                openButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "ALREADY OPENED";
                    statusText.color = Color.gray;
                }
            }
            else if (cost > 0 && ScoreManager.Instance != null && ScoreManager.Instance.Points < cost)
            {
                openButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "NOT ENOUGH POINTS";
                    statusText.color = Color.red;
                }
            }
            else
            {
                openButton.interactable = true;
                if (statusText != null)
                {
                    statusText.text = "Press E or Click to Open";
                    statusText.color = Color.green;
                }
            }
        }
    }

    public void OnOpenButton()
    {
        if (lootChest != null && player != null)
        {
            bool success = lootChest.Interact(player);
            
            if (success)
            {
                UpdateUI();
                // Optional: Add success sound/effect here
                Debug.Log("[MysteryChestUI] Chest opened successfully!");
                
                // Optional: Show animation or special effects
                StartCoroutine(ShowOpenAnimation());
            }
            else
            {
                // Optional: Add failure sound/effect here
                Debug.Log("[MysteryChestUI] Failed to open chest");
            }
        }
    }

    private System.Collections.IEnumerator ShowOpenAnimation()
    {
        // Optional: Add visual feedback when chest is opened
        // For now, just wait and update UI
        yield return new UnityEngine.WaitForSeconds(0.5f);
        UpdateUI();
    }
}
