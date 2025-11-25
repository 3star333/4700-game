using UnityEngine;

public enum PerkType
{
    DoubleDamage,
    SpeedBoost,
    ExtraHealth
}

public class PerkStation : MonoBehaviour, IInteractable
{
    [SerializeField] private PerkType type = PerkType.DoubleDamage;
    [SerializeField] private int cost = 500;
    [SerializeField] private float multiplier = 1.5f; // Effect, meaning depends on perk type
    [SerializeField] private bool purchased = false;

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
                BaseWeapon bw = interactor.GetComponentInChildren<BaseWeapon>();
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
}
