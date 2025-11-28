using UnityEngine;
using UnityEngine.InputSystem;

public class Longsword : MeleeWeapon
{
    [Header("Melee Settings")]
    public MeleeHitbox hitbox;

    [Header("Block Settings")]
    public float blockDamageReduction = 0.5f; // 50% damage reduced

    private PlayerDefense playerDefense;

    private void Awake()
    {
        // Find or add PlayerDefense on the owning player
        playerDefense = GetComponentInParent<PlayerDefense>();
        if (playerDefense == null && transform.root != null)
        {
            playerDefense = transform.root.gameObject.GetComponent<PlayerDefense>();
            if (playerDefense == null)
                playerDefense = transform.root.gameObject.AddComponent<PlayerDefense>();
        }

        if (playerDefense != null)
            playerDefense.SetBlockMultiplier(blockDamageReduction);
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (hitbox == null) return;
        DoMelee(damage);
    }

    // Block input toggles are handled via PlayerDefense but Longsword ensures the multiplier is set.
    public void OnBlock(InputAction.CallbackContext context)
    {
        if (playerDefense == null) return;
        if (context.performed)
            playerDefense.SetBlocking(true);
        else if (context.canceled)
            playerDefense.SetBlocking(false);
    }
}
