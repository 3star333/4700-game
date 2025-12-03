using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Holds and manages player weapons, supports adding new weapons at runtime and switching.
/// Attach to Player GameObject.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform weaponMount;
    private List<Weapon> ownedWeapons = new List<Weapon>();
    private int currentIndex = -1;

    [Header("Debug / Starting Weapons")]
    [Tooltip("If true and there are no weapons on Start, the debug weapon prefab (or starting list) will be granted.")]
    [SerializeField] private bool grantDebugWeaponOnStart = true;

    [Tooltip("Optional single debug starting weapon prefab to auto-give on Play.")]
    [SerializeField] private GameObject debugStartingWeaponPrefab;

    [Tooltip("Optional list of starting weapon prefabs to grant on Play (first is auto-equipped).")]
    [SerializeField] private List<GameObject> startingWeaponPrefabs = new List<GameObject>();

    public Weapon CurrentWeapon => currentIndex >= 0 && currentIndex < ownedWeapons.Count ? ownedWeapons[currentIndex] : null;

    public event Action<Weapon> OnWeaponEquipped; // fired after a weapon becomes active

    private void Awake()
    {
        // Ensure we have a mount transform; create one if missing.
        if (weaponMount == null)
        {
            var mountGO = new GameObject("WeaponMount");
            mountGO.transform.SetParent(transform);
            mountGO.transform.localPosition = Vector3.zero;
            mountGO.transform.localRotation = Quaternion.identity;
            weaponMount = mountGO.transform;
        }
    }

    private void Start()
    {
        // Grant starting/debug weapons if none are owned yet.
        if (ownedWeapons.Count == 0 && grantDebugWeaponOnStart)
        {
            if (startingWeaponPrefabs != null && startingWeaponPrefabs.Count > 0)
            {
                for (int i = 0; i < startingWeaponPrefabs.Count; i++)
                {
                    var pf = startingWeaponPrefabs[i];
                    if (pf != null)
                        AddWeapon(pf);
                }
                if (ownedWeapons.Count > 0)
                    EquipWeapon(0);
            }
            else if (debugStartingWeaponPrefab != null)
            {
                AddWeapon(debugStartingWeaponPrefab);
            }
        }
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;
        GameObject newWeapon = Instantiate(weaponPrefab, weaponMount != null ? weaponMount : transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

    // Lower view instantly if the weapon has a WeaponView component
    newWeapon.SendMessage("SetLoweredInstant", SendMessageOptions.DontRequireReceiver);

        Weapon baseWeapon = newWeapon.GetComponent<Weapon>();
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
            Debug.LogWarning("Added weapon prefab missing a Weapon component");
            Destroy(newWeapon);
        }
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= ownedWeapons.Count) return;

        if (currentIndex >= 0 && currentIndex < ownedWeapons.Count)
            ownedWeapons[currentIndex].gameObject.SetActive(false);

        currentIndex = index;
    GameObject go = ownedWeapons[currentIndex].gameObject;
    go.SetActive(true);

    // play equip animation if present
    go.SendMessage("PlayEquip", SendMessageOptions.DontRequireReceiver);

        OnWeaponEquipped?.Invoke(ownedWeapons[currentIndex]);
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

    public void AddOwnedWeapon(Weapon w)
    {
        if (w == null) return;
        ownedWeapons.Add(w);
        if (ownedWeapons.Count == 1)
            EquipWeapon(0);
        else
        {
            w.gameObject.SetActive(false);
            w.gameObject.SendMessage("SetLoweredInstant", SendMessageOptions.DontRequireReceiver);
        }
    }

    // Debug helper to grant a weapon at runtime (can be called from UI or console)
    public void GiveWeapon(GameObject weaponPrefab)
    {
        AddWeapon(weaponPrefab);
    }

    [ContextMenu("Give Debug Weapon Now")] // Right-click component in Inspector during Play
    private void ContextGiveDebugWeaponNow()
    {
        if (debugStartingWeaponPrefab != null)
        {
            AddWeapon(debugStartingWeaponPrefab);
        }
        else if (startingWeaponPrefabs != null && startingWeaponPrefabs.Count > 0)
        {
            AddWeapon(startingWeaponPrefabs[0]);
        }
        else
        {
            Debug.LogWarning("No debugStartingWeaponPrefab or startingWeaponPrefabs configured.");
        }
    }

    // Input forwarding so PlayerInput on the Player can drive the active Weapon component on a child.
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (CurrentWeapon == null) return;
        // Forward press/release accurately so auto fire stops on button up
        if (ctx.canceled)
        {
            CurrentWeapon.SendMessage("OnAttackCanceled", ctx, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            CurrentWeapon.SendMessage("OnAttack", ctx, SendMessageOptions.DontRequireReceiver);
        }
    }

    // Some Send Messages setups invoke the InputValue overload.
    public void OnAttack(InputValue value)
    {
        if (CurrentWeapon == null) return;
        if (value.isPressed)
        {
            CurrentWeapon.SendMessage("OnAttack", value, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            CurrentWeapon.SendMessage("OnAttackCanceled", value, SendMessageOptions.DontRequireReceiver);
        }
    }

    // Fallback (rare) parameterless call – trigger a single shot if possible.
    public void OnAttack()
    {
        if (CurrentWeapon == null) return;
        CurrentWeapon.SendMessage("OnAttack", SendMessageOptions.DontRequireReceiver);
    }

    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (CurrentWeapon == null) return;
        CurrentWeapon.SendMessage("OnReload", ctx, SendMessageOptions.DontRequireReceiver);
    }

    public void OnReload(InputValue value)
    {
        if (CurrentWeapon == null) return;
        CurrentWeapon.SendMessage("OnReload", value, SendMessageOptions.DontRequireReceiver);
    }

    public void OnReload()
    {
        if (CurrentWeapon == null) return;
        CurrentWeapon.SendMessage("OnReload", SendMessageOptions.DontRequireReceiver);
    }
}

