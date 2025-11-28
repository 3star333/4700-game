using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Controller for Pack-a-Punch panel.
/// Displays weapon upgrade information and handles upgrade button.
/// </summary>
public class PackAPunchUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI playerPointsText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Pack-a-Punch Reference")]
    [SerializeField] private PackAPunch packAPunch;

    private GameObject player;

    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButton);
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

    public void SetPackAPunch(PackAPunch pap, GameObject playerObject)
    {
        packAPunch = pap;
        player = playerObject;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (packAPunch == null) return;

        // Get Pack-a-Punch info using reflection
        var costField = packAPunch.GetType().GetField("cost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var multiplierField = packAPunch.GetType().GetField("damageMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var usedField = packAPunch.GetType().GetField("used", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        int cost = (int)costField.GetValue(packAPunch);
        float multiplier = (float)multiplierField.GetValue(packAPunch);
        bool used = (bool)usedField.GetValue(packAPunch);

        // Update title
        if (titleText != null)
        {
            titleText.text = "PACK-A-PUNCH";
        }

        // Get current weapon name
        string weaponName = "No Weapon Equipped";
        if (player != null)
        {
            WeaponManager wm = player.GetComponent<WeaponManager>();
            if (wm != null && wm.CurrentWeapon != null)
            {
                weaponName = wm.CurrentWeapon.gameObject.name.Replace("(Clone)", "").Trim();
            }
            else
            {
                Weapon weapon = player.GetComponentInChildren<Weapon>();
                if (weapon != null)
                {
                    weaponName = weapon.gameObject.name.Replace("(Clone)", "").Trim();
                }
            }
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

        // Update multiplier
        if (multiplierText != null)
        {
            multiplierText.text = $"Damage Multiplier: x{multiplier:F1}";
        }

        // Update description
        if (descriptionText != null)
        {
            descriptionText.text = "Upgrade your current weapon\nwith enhanced damage and effects";
        }

        // Update player points
        if (playerPointsText != null && ScoreManager.Instance != null)
        {
            playerPointsText.text = $"Your Points: {ScoreManager.Instance.Points}";
        }

        // Update button state
        if (upgradeButton != null)
        {
            if (used)
            {
                upgradeButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "ALREADY UPGRADED";
                    statusText.color = Color.yellow;
                }
            }
            else if (weaponName == "No Weapon Equipped")
            {
                upgradeButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "NO WEAPON EQUIPPED";
                    statusText.color = Color.gray;
                }
            }
            else if (ScoreManager.Instance != null && ScoreManager.Instance.Points < cost)
            {
                upgradeButton.interactable = false;
                if (statusText != null)
                {
                    statusText.text = "NOT ENOUGH POINTS";
                    statusText.color = Color.red;
                }
            }
            else
            {
                upgradeButton.interactable = true;
                if (statusText != null)
                {
                    statusText.text = "Press E or Click to Upgrade";
                    statusText.color = Color.green;
                }
            }
        }
    }

    public void OnUpgradeButton()
    {
        if (packAPunch != null && player != null)
        {
            bool success = packAPunch.Interact(player);
            
            if (success)
            {
                UpdateUI();
                // Optional: Add success sound/effect here
                Debug.Log("[PackAPunchUI] Weapon upgraded successfully!");
            }
            else
            {
                // Optional: Add failure sound/effect here
                Debug.Log("[PackAPunchUI] Failed to upgrade weapon");
            }
        }
    }
}
