using System.Collections;
using UnityEngine;

/// <summary>
/// Simple melee hitbox. When activated, it checks for hits in a short cone and applies damage/status.
/// </summary>
public class MeleeHitbox : MonoBehaviour
{
    public float radius = 1.2f;
    public float angle = 60f;
    public LayerMask hitMask;

    public void PerformMelee(float damage, System.Action<GameObject> onHit = null)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, hitMask);
        foreach (var c in hits)
        {
            Vector3 dir = (c.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) <= angle * 0.5f)
            {
                // Call onHit so weapon-specific logic can run
                onHit?.Invoke(c.gameObject);
            }
        }
    }
}
