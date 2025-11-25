using UnityEngine;

public class WeaponBuyStation : MonoBehaviour, IInteractable
{
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private int cost = 500;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private bool purchased = false;

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

            // If a weapon prefab has a WeaponController component, instantiate and parent it to the player
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
}
