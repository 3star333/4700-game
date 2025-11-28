using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Controller for Weapon Buy Station panel.
/// Displays weapon information and handles purchase button.
/// </summary>
public class WeaponBuyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI playerPointsText;
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Weapon Buy Station Reference")]
    [SerializeField] private WeaponBuyStation weaponBuyStation;

    [Header("Optional Icon")]
    [SerializeField] private Sprite weaponIcon;

    private GameObject player;

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButton);
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

    public void SetWeaponBuyStation(WeaponBuyStation station, GameObject playerObject)
    {
        weaponBuyStation = station;
        player = playerObject;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (weaponBuyStation == null) return;

        // Get weapon buy station info using reflection
        var nameField = weaponBuyStation.GetType().GetField("weaponName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var costField = weaponBuyStation.GetType().GetField("cost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var purchasedField = weaponBuyStation.GetType().GetField("purchased", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefabField = weaponBuyStation.GetType().GetField("weaponPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        string weaponName = (string)nameField.GetValue(weaponBuyStation);
        int cost = (int)costField.GetValue(weaponBuyStation);
        bool purchased = (bool)purchasedField.GetValue(weaponBuyStation);
        GameObject weaponPrefab = (GameObject)prefabField.GetValue(weaponBuyStation);

        // Update title
        if (titleText != null)
        {
            titleText.text = "WEAPON BUY STATION";
        }

        // Update weapon name
        if (weaponNameText != null)
        {
            weaponNameText.text = weaponName;
        }

        // Update cost
        if (costText != null)
        {
            costText.text = $"Cost: {cost} Points";
        }

        // Update description (get from weapon if available)
        if (descriptionText != null)
        {
            string description = GetWeaponDescription(weaponPrefab);
            descriptionText.text = description;
        }

        // Update weapon icon
        if (weaponIconImage != null)
        {
            if (weaponIcon != null)
            {
                weaponIconImage.sprite = weaponIcon;
                weaponIconImage.enabled = true;
            }
            else
            {
                weaponIconImage.enabled = false;
            }
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

    private string GetWeaponDescription(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            return "Weapon details unavailable";
        }

        // Try to get weapon stats from the prefab
        Weapon weapon = weaponPrefab.GetComponent<Weapon>();
        if (weapon != null)
        {
            // Use reflection to get damage value
            var damageField = weapon.GetType().GetField("damage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (damageField != null)
            {
                int damage = (int)damageField.GetValue(weapon);
                return $"A powerful weapon for your arsenal\nBase Damage: {damage}";
            }
        }

        return "A powerful weapon for your arsenal";
    }

    public void OnBuyButton()
    {
        if (weaponBuyStation != null && player != null)
        {
            bool success = weaponBuyStation.Interact(player);
            
            if (success)
            {
                UpdateUI();
                // Optional: Add success sound/effect here
                Debug.Log("[WeaponBuyUI] Weapon purchased successfully!");
            }
            else
            {
                // Optional: Add failure sound/effect here
                Debug.Log("[WeaponBuyUI] Failed to purchase weapon");
            }
        }
    }

    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon = icon;
        if (weaponIconImage != null)
        {
            weaponIconImage.sprite = icon;
            weaponIconImage.enabled = icon != null;
        }
    }
}
