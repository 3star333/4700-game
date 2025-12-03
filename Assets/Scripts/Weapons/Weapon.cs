using UnityEngine;
using UnityEngine.InputSystem;
using GothicShooter.Core;
using System;
using GothicShooter.Mobs;

/// <summary>
/// Base Weapon class with built-in hitscan firing.
/// Concrete guns inherit and configure stats; no need to override Fire().
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Weapon";
    public float damage = 10f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    public int currentAmmo = 0;

    [Header("Hitscan Settings")]
    public float range = 100f;
    public int pelletsPerShot = 1; // for shotguns; 1 for rifles
    public float spreadAngle = 0f; // cone spread in degrees

    [Header("References")]
    public Transform firePoint;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip emptySound;

    [Header("Input")]
    public bool isAutomatic = true;

    [Header("Reload")]
    [Tooltip("Seconds to reload a full magazine.")]
    public float reloadTime = 1.6f;
    public AudioClip reloadSound;

    [Header("Ammo")]
    [Tooltip("Rounds in reserve in addition to the magazine.")]
    public int reserveAmmo = 60; // default extra ammo
    [Tooltip("Show debug ammo counters on screen.")]
    public bool showDebugAmmo = true;

    [Header("Hit Filtering")]
    [Tooltip("Collision layers this weapon can hit.")]
    public LayerMask hitMask = ~0; // default: everything
    [Tooltip("Skip the owner/player colliders so we never hurt ourselves.")]
    public bool ignoreOwner = true;
    [Tooltip("Ignore trigger colliders in hitscan.")]
    public bool ignoreTriggers = true;

    protected float nextFireTime = 0f;
    protected Camera playerCamera;
    private bool shootInput = false;
    private Transform ownerRoot;
    private Collider[] ownerColliders;
    // Fallback: track last input event frame to avoid latching when release isn't delivered
    private int lastAttackEventFrame = -1000;
    private bool isReloading = false;

    public virtual void Start()
    {
        currentAmmo = maxAmmo;
        playerCamera = Camera.main;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ownerRoot = transform.root;
        if (ignoreOwner && ownerRoot != null)
        {
            ownerColliders = ownerRoot.GetComponentsInChildren<Collider>(true);
        }
    }

    private void Update()
    {
        // Safety: if automatic is latched but we didn't receive any press event this frame, stop
        if (isAutomatic && shootInput && (Time.frameCount - lastAttackEventFrame) > 1)
        {
            shootInput = false;
        }

        // Handle shooting
        if (shootInput && CanFire())
        {
            Fire();
        }
        else if (shootInput && currentAmmo <= 0)
        {
            if (audioSource && emptySound)
                audioSource.PlayOneShot(emptySound);
            // Optional auto-reload when trying to shoot empty
            if (!isReloading && reserveAmmo > 0)
                Reload();
        }
    }

    public virtual void Fire()
    {
        if (!CanFire()) return;

    Debug.Log($"[Weapon] Fired {weaponName} at {Time.time:F2}s | mag: {currentAmmo}/{maxAmmo} | reserve: {reserveAmmo} | total: {currentAmmo + reserveAmmo}");

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        // Effects
        if (muzzleFlash)
            muzzleFlash.Play();

        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        // Hitscan per pellet
        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shootDir = GetShootDirection();
            DoHitscan(shootDir);
        }
    }

    protected virtual Vector3 GetShootDirection()
    {
        // Origin at muzzle (if available) to keep effects correct
        Vector3 origin = firePoint != null ? firePoint.position : (playerCamera != null ? playerCamera.transform.position : transform.position);

        // Aim using camera center ray (crosshair)
        Vector3 targetPoint;
        if (playerCamera != null)
        {
            Ray camRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(camRay, out RaycastHit camHit, range, hitMask, ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide))
            {
                targetPoint = camHit.point;
            }
            else
            {
                targetPoint = playerCamera.transform.position + playerCamera.transform.forward * range;
            }
        }
        else
        {
            targetPoint = origin + transform.forward * range;
        }

        Vector3 baseDir = (targetPoint - origin).normalized;

        if (spreadAngle > 0f)
        {
            float spreadRad = spreadAngle * Mathf.Deg2Rad;
            float randomX = UnityEngine.Random.Range(-spreadRad, spreadRad);
            float randomY = UnityEngine.Random.Range(-spreadRad, spreadRad);
            Quaternion spread = Quaternion.Euler(randomX * Mathf.Rad2Deg, randomY * Mathf.Rad2Deg, 0f);
            baseDir = spread * baseDir;
        }

        return baseDir.normalized;
    }

    protected virtual void DoHitscan(Vector3 direction)
    {
        Vector3 origin = playerCamera != null ? playerCamera.transform.position : transform.position;
        if (firePoint != null)
            origin = firePoint.position;

        // Nudge origin slightly forward to avoid starting inside owner collider
        origin += direction * 0.01f;

        var ray = new Ray(origin, direction);
        var hits = Physics.RaycastAll(
            ray,
            range,
            hitMask,
            ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        bool applied = false;

        if (hits != null && hits.Length > 0)
        {
            // Sort by distance to get the first valid non-owner hit
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                // Skip owner colliders
                if (ignoreOwner)
                {
                    if (ownerColliders != null)
                    {
                        for (int i = 0; i < ownerColliders.Length; i++)
                        {
                            if (hit.collider == ownerColliders[i])
                            {
                                goto ContinueLoop; // skip to next hit
                            }
                        }
                    }
                    // Extra safety: skip anything with the same root
                    if (hit.collider.transform.root == ownerRoot)
                    {
                        goto ContinueLoop;
                    }
                }

                Debug.Log($"[Weapon] Hit: {hit.collider.name}");

                // Apply damage to the first non-owner damageable found (prefer parent search)
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage, gameObject);

                    // Debug hitmarker when hitting wolves
                    if (hit.collider.GetComponentInParent<WolfAI>() != null)
                    {
                        Debug.Log("[Hitmarker] Wolf hit!");
                    }

                    if (impactEffect != null)
                    {
                        GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(impact, 2f);
                    }

                    Debug.DrawRay(origin, direction * hit.distance, Color.red, 1f);
                    applied = true;
                    break; // only process first valid hit
                }

            ContinueLoop:;
            }
        }

        // Fallback pass: include triggers to support trigger-only enemy hitboxes
        if (!applied)
        {
            var hits2 = Physics.RaycastAll(
                ray,
                range,
                hitMask,
                QueryTriggerInteraction.Collide
            );

            if (hits2 != null && hits2.Length > 0)
            {
                Array.Sort(hits2, (a, b) => a.distance.CompareTo(b.distance));
                foreach (var hit in hits2)
                {
                    if (ignoreOwner)
                    {
                        if (ownerColliders != null)
                        {
                            for (int i = 0; i < ownerColliders.Length; i++)
                            {
                                if (hit.collider == ownerColliders[i])
                                {
                                    goto ContinueLoop2;
                                }
                            }
                        }
                        if (hit.collider.transform.root == ownerRoot)
                        {
                            goto ContinueLoop2;
                        }
                    }

                    IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage, gameObject);
                        if (hit.collider.GetComponentInParent<WolfAI>() != null)
                        {
                            Debug.Log("[Hitmarker] Wolf hit! (trigger)");
                        }
                        // We skip impact VFX on trigger hits to avoid weird normals
                        Debug.DrawRay(origin, direction * hit.distance, Color.yellow, 1f);
                        applied = true;
                        break;
                    }

                ContinueLoop2:;
                }
            }
        }
    }

    // Simple built-in debug crosshair drawn at screen center (where hitscan aims)
    private void OnGUI()
    {
        // Minimal crosshair: small plus sign at screen center
        if (Event.current.type != EventType.Repaint) return;
        var size = 8f;
        var gap = 2f;
        var color = Color.white;

        // Use GUI drawing to render lines
        var w = Screen.width;
        var h = Screen.height;
        float cx = w * 0.5f;
        float cy = h * 0.5f;

        // Horizontal line
        DrawLine(new Vector2(cx - size - gap, cy), new Vector2(cx - gap, cy), color, 1f);
        DrawLine(new Vector2(cx + gap, cy), new Vector2(cx + size + gap, cy), color, 1f);
        // Vertical line
        DrawLine(new Vector2(cx, cy - size - gap), new Vector2(cx, cy - gap), color, 1f);
        DrawLine(new Vector2(cx, cy + gap), new Vector2(cx, cy + size + gap), color, 1f);

        // Debug ammo counters (bottom-right of crosshair)
        if (showDebugAmmo)
        {
            var label = $"{weaponName}: {currentAmmo}/{maxAmmo} | {reserveAmmo}";
            var content = new GUIContent(label);
            var style = GUI.skin.label;
            var size2 = style.CalcSize(content);
            var pos = new Rect(cx + 14, cy + 12, size2.x + 8, size2.y + 4);
            var old = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.Box(pos, GUIContent.none);
            GUI.color = Color.white;
            GUI.Label(new Rect(pos.x + 4, pos.y + 2, pos.width - 8, pos.height - 4), label);
            GUI.color = old;
        }
    }

    // Lightweight line drawing using GL (avoids textures). Debug-only.
    private static Material _lineMat;
    private static void EnsureLineMaterial()
    {
        if (_lineMat != null) return;
        var shader = Shader.Find("Hidden/Internal-Colored");
        _lineMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        _lineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _lineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _lineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        _lineMat.SetInt("_ZWrite", 0);
    }

    private static void DrawLine(Vector2 a, Vector2 b, Color color, float thickness)
    {
        EnsureLineMaterial();
        _lineMat.SetPass(0);
        GL.PushMatrix();
        GL.LoadPixelMatrix();
        GL.Begin(GL.QUADS);
        GL.Color(color);
        // Build a 2D quad for the line
        Vector2 dir = (b - a).normalized;
        Vector2 n = new Vector2(-dir.y, dir.x) * thickness;
        GL.Vertex3(a.x - n.x, a.y - n.y, 0);
        GL.Vertex3(a.x + n.x, a.y + n.y, 0);
        GL.Vertex3(b.x + n.x, b.y + n.y, 0);
        GL.Vertex3(b.x - n.x, b.y - n.y, 0);
        GL.End();
        GL.PopMatrix();
    }

    public virtual void Reload()
    {
        if (isReloading) return;
        if (currentAmmo >= maxAmmo) return; // already full
    if (reserveAmmo <= 0) return; // nothing to reload with
        StartCoroutine(ReloadRoutine());
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log($"[Weapon] {weaponName} reloading...");
        if (audioSource && reloadSound)
            audioSource.PlayOneShot(reloadSound);
        float t = 0f;
        while (t < reloadTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
    int needed = maxAmmo - currentAmmo;
    int taken = Mathf.Clamp(needed, 0, reserveAmmo);
    currentAmmo += taken;
    reserveAmmo -= taken;
        isReloading = false;
    Debug.Log($"[Weapon] {weaponName} reloaded | mag: {currentAmmo}/{maxAmmo} | reserve: {reserveAmmo} | total: {currentAmmo + reserveAmmo}");
    }

    public virtual bool CanFire()
    {
        return !isReloading && Time.time >= nextFireTime && currentAmmo > 0;
    }

    // Input callbacks
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isAutomatic)
        {
            if (context.started)
                shootInput = true;
            else if (context.canceled)
                shootInput = false;
            lastAttackEventFrame = Time.frameCount;
        }
        else
        {
            // Semi-auto: fire only on performed, never latch
            if (context.performed)
                Fire();
            if (context.canceled)
                shootInput = false;
            lastAttackEventFrame = Time.frameCount;
        }
    }

    public void OnAttack(InputValue value)
    {
        if (isAutomatic)
        {
            shootInput = value.isPressed;
            lastAttackEventFrame = Time.frameCount;
        }
        else
        {
            if (value.isPressed)
            {
                Fire();
            }
            else
            {
                shootInput = false;
            }
            lastAttackEventFrame = Time.frameCount;
        }
    }

    // Fallback when Send Messages invokes without parameters
    public void OnAttack()
    {
    // Simulate a one-frame press to avoid latching in parameterless mode
    if (!gameObject.activeInHierarchy) return;
    StartCoroutine(OneFramePress());
    lastAttackEventFrame = Time.frameCount;
    }

    // Release handlers ensure we stop continuous fire on button up in any message mode
    public void OnAttackCanceled(InputAction.CallbackContext context)
    {
        shootInput = false;
    lastAttackEventFrame = Time.frameCount;
    }
    public void OnAttackCanceled(InputValue value)
    {
        shootInput = false;
    lastAttackEventFrame = Time.frameCount;
    }
    public void OnAttackCanceled()
    {
        shootInput = false;
    lastAttackEventFrame = Time.frameCount;
    }

    private System.Collections.IEnumerator OneFramePress()
    {
        shootInput = true;
        // wait one frame
        yield return null;
        shootInput = false;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) Reload();
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed) Reload();
    }

    // Fallback when Send Messages invokes without parameters
    public void OnReload()
    {
        Reload();
    }

    // Upgrades
    public virtual void ApplyPackAPunch(float damageMultiplier = 2f)
    {
        damage *= damageMultiplier;
    }

    public virtual void ModifyDamage(float multiplier)
    {
        damage *= multiplier;
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

    /// <summary>
    /// Add ammo to this weapon's reserve pool. Used by pickups and rewards.
    /// </summary>
    public void AddReserveAmmo(int amount)
    {
        if (amount <= 0) return;
        reserveAmmo += amount;
    }
}
