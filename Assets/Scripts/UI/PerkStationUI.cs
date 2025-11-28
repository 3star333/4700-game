using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Controller for Perk Station panel.
/// Uses STUBS since perk system is not fully implemented.
/// Displays perk name, cost, and placeholder information.
/// </summary>
public class PerkStationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI perkNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI playerPointsText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Perk Station Reference")]
    [SerializeField] private PerkStation perkStation;

    private GameObject player;

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyPerkButton);
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

    public void SetPerkStation(PerkStation station, GameObject playerObject)
    {
        perkStation = station;
        player = playerObject;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (perkStation == null) return;

        // Get perk info using reflection (since fields are private)
        var typeField = perkStation.GetType().GetField("type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var costField = perkStation.GetType().GetField("cost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var purchasedField = perkStation.GetType().GetField("purchased", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        PerkType perkType = (PerkType)typeField.GetValue(perkStation);
        int cost = (int)costField.GetValue(perkStation);
        bool purchased = (bool)purchasedField.GetValue(perkStation);

        // Update title
        if (titleText != null)
        {
            titleText.text = "PERK STATION";
        }

        // Update perk name
        if (perkNameText != null)
        {
            perkNameText.text = GetPerkDisplayName(perkType);
        }

        // Update cost
        if (costText != null)
        {
            costText.text = $"Cost: {cost} Points";
        }

        // Update description (STUB - placeholders)
        if (descriptionText != null)
        {
            descriptionText.text = GetPerkDescription(perkType);
        }

        // Update player points
        if (playerPointsText != null && ScoreManager.Instance != null)
        {
            playerPointsText.text = $"Your Points: {ScoreManager.Instance.Points}";
        }

        // Update button state
        if (buyButton != null)
        {
            if (purchased)
            {
                buyButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "ALREADY PURCHASED";
                    statusText.color = Color.yellow;
                }
            }
            else if (ScoreManager.Instance != null && ScoreManager.Instance.Points < cost)
            {
                buyButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "NOT ENOUGH POINTS";
                    statusText.color = Color.red;
                }
            }
            else
            {
                buyButton.interactable = true;
                if (statusText != null)
                {
                    statusText.text = "Press E or Click to Buy";
                    statusText.color = Color.green;
                }
            }
        }
    }

    private string GetPerkDisplayName(PerkType type)
    {
        switch (type)
        {
            case PerkType.DoubleDamage:
                return "Double Tap";
            case PerkType.SpeedBoost:
                return "Quick Revive";
            case PerkType.ExtraHealth:
                return "Juggernog";
            default:
                return "Unknown Perk";
        }
    }

    private string GetPerkDescription(PerkType type)
    {
        // STUB: Placeholder descriptions since perk system is not fully implemented
        switch (type)
        {
            case PerkType.DoubleDamage:
                return "[STUB] Increases weapon damage\nPerk effects partially implemented";
            case PerkType.SpeedBoost:
                return "[STUB] Increases movement speed\nPerk system not yet implemented";
            case PerkType.ExtraHealth:
                return "[STUB] Increases maximum health\nPerk system not yet implemented";
            default:
                return "[STUB] Perk effect not defined";
        }
    }

    public void OnBuyPerkButton()
    {
        if (perkStation != null && player != null)
        {
            bool success = perkStation.Interact(player);
            
            if (success)
            {
                UpdateUI();
                // Optional: Add success sound/effect here
                Debug.Log("[PerkStationUI] Perk purchased successfully!");
            }
            else
            {
                // Optional: Add failure sound/effect here
                Debug.Log("[PerkStationUI] Failed to purchase perk");
            }
        }
    }

    /// <summary>
    /// STUB method for future perk system expansion
    /// </summary>
    public void OnBuyPerkStub()
    {
        Debug.Log("[STUB] Perk system not fully implemented yet. This is a placeholder.");
    }
}
