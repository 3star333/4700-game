using UnityEngine;

public class PackAPunch : MonoBehaviour, IInteractable
{
    [SerializeField] private int cost = 1000;
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private bool used = false;

    public bool Interact(GameObject interactor)
    {
        if (used)
        {
            Debug.Log("Pack-A-Punch already used.");
            return false;
        }

        if (ScoreManager.Instance == null)
            return false;

        if (!ScoreManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points for Pack-A-Punch.");
            return false;
        }

        // Find the player's WeaponController and apply upgrade
        WeaponController wc = interactor.GetComponentInChildren<WeaponController>();
        if (wc != null)
        {
            wc.ApplyPackAPunch(damageMultiplier);
            used = true;
            Debug.Log("Pack-A-Punch applied to player's weapon.");
            return true;
        }

        Debug.LogWarning("Player has no WeaponController to upgrade.");
        return false;
    }
}
