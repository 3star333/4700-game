using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple HUD controller that shows icons for each perk the player owns.
/// Attach this to a UI GameObject under your in-game HUD Canvas.
/// </summary>
public class PerkHUD : MonoBehaviour
{
    [System.Serializable]
    public class PerkIconConfig
    {
        public PerkType type;
        public Sprite icon;
    }

    [Header("Icon Setup")]
    [SerializeField] private Transform iconContainer;
    [SerializeField] private GameObject perkIconPrefab;
    [SerializeField] private PerkIconConfig[] perkIcons;

    private readonly Dictionary<PerkType, GameObject> spawnedIcons = new Dictionary<PerkType, GameObject>();
    private readonly Dictionary<PerkType, Sprite> iconLookup = new Dictionary<PerkType, Sprite>();

    private PlayerPerkTracker tracker;

    private void Awake()
    {
        // Build a lookup PerkType -> Sprite
        if (perkIcons != null)
        {
            foreach (var config in perkIcons)
            {
                if (config == null) continue;
                if (!iconLookup.ContainsKey(config.type) && config.icon != null)
                {
                    iconLookup.Add(config.type, config.icon);
                }
            }
        }
    }

    private void Start()
    {
        // Find the player and their perk tracker
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[PerkHUD] No GameObject with tag 'Player' found.");
            return;
        }

        tracker = player.GetComponent<PlayerPerkTracker>();
        if (tracker == null)
        {
            Debug.LogWarning("[PerkHUD] PlayerPerkTracker component not found on Player.");
            return;
        }

        tracker.OnPerkAdded += HandlePerkAdded;

        // Initialize HUD with any already-owned perks
        foreach (var perk in tracker.GetOwnedPerks())
        {
            HandlePerkAdded(perk);
        }
    }

    private void OnDestroy()
    {
        if (tracker != null)
        {
            tracker.OnPerkAdded -= HandlePerkAdded;
        }
    }

    private void HandlePerkAdded(PerkType type)
    {
        if (spawnedIcons.ContainsKey(type))
            return;

        if (iconContainer == null || perkIconPrefab == null)
        {
            Debug.LogWarning("[PerkHUD] Icon container or prefab not assigned.");
            return;
        }

        GameObject iconGO = Instantiate(perkIconPrefab, iconContainer);
        spawnedIcons[type] = iconGO;

        // Assign sprite if available
        Image img = iconGO.GetComponentInChildren<Image>();
        if (img != null && iconLookup.TryGetValue(type, out Sprite sprite))
        {
            img.sprite = sprite;
        }
    }
}
