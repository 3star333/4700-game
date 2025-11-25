using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds and manages player weapons, supports adding new weapons at runtime and switching.
/// Attach to Player GameObject.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform weaponMount;
    private List<BaseWeapon> ownedWeapons = new List<BaseWeapon>();
    private int currentIndex = -1;

    public BaseWeapon CurrentWeapon => currentIndex >= 0 && currentIndex < ownedWeapons.Count ? ownedWeapons[currentIndex] : null;

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;
        GameObject newWeapon = Instantiate(weaponPrefab, weaponMount != null ? weaponMount : transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        BaseWeapon baseWeapon = newWeapon.GetComponent<BaseWeapon>();
        if (baseWeapon != null)
        {
            ownedWeapons.Add(baseWeapon);
            if (ownedWeapons.Count == 1)
            {
                EquipWeapon(0);
            }
            else
            {
                // Disable newly added weapon until equipped
                newWeapon.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Added weapon prefab missing a BaseWeapon component");
            Destroy(newWeapon);
        }
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= ownedWeapons.Count) return;

        if (currentIndex >= 0 && currentIndex < ownedWeapons.Count)
            ownedWeapons[currentIndex].gameObject.SetActive(false);

        currentIndex = index;
        ownedWeapons[currentIndex].gameObject.SetActive(true);
    }

    public void EquipNextWeapon()
    {
        if (ownedWeapons.Count <= 1) return;
        int next = (currentIndex + 1) % ownedWeapons.Count;
        EquipWeapon(next);
    }

    public void ApplyPackAPunchToCurrent(float damageMultiplier = 2f)
    {
        var c = CurrentWeapon;
        if (c != null)
        {
            c.ApplyPackAPunch(damageMultiplier);
        }
    }

    public void AddOwnedWeapon(BaseWeapon w)
    {
        if (w == null) return;
        ownedWeapons.Add(w);
        if (ownedWeapons.Count == 1)
            EquipWeapon(0);
        else
            w.gameObject.SetActive(false);
    }
}

