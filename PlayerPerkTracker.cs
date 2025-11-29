using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks which perks the player has purchased.
/// Lives on the Player GameObject and is used by UI (PerkHUD).
/// </summary>
public class PlayerPerkTracker : MonoBehaviour
{
    [Tooltip("Optional starting perks for debugging or special modes.")]
    [SerializeField] private List<PerkType> startingPerks = new List<PerkType>();

    private readonly HashSet<PerkType> ownedPerks = new HashSet<PerkType>();

    /// <summary>
    /// Fired when a new perk is added (not fired for duplicates).
    /// </summary>
    public event Action<PerkType> OnPerkAdded;

    private void Awake()
    {
        // Initialize with any starting perks
        foreach (var perk in startingPerks)
        {
            RegisterPerk(perk);
        }
    }

    /// <summary>
    /// Returns true if the player already has this perk.
    /// </summary>
    public bool HasPerk(PerkType perkType)
    {
        return ownedPerks.Contains(perkType);
    }

    /// <summary>
    /// Register a perk as owned. Returns true if this is a new perk.
    /// </summary>
    public bool RegisterPerk(PerkType perkType)
    {
        if (ownedPerks.Contains(perkType))
        {
            // Already have this perk; don't re-fire event
            return false;
        }

        ownedPerks.Add(perkType);
        OnPerkAdded?.Invoke(perkType);
        return true;
    }

    /// <summary>
    /// Read-only access to all owned perks. Useful for initializing HUD.
    /// </summary>
    public IReadOnlyCollection<PerkType> GetOwnedPerks()
    {
        return ownedPerks;
    }
}
