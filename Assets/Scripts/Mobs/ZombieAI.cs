using UnityEngine;
using GothicShooter.Health;
namespace GothicShooter.Mobs
{

private EnemyAudio enemyAudio;

void Awake()
{
    enemyAudio = GetComponent<EnemyAudio>();
}

void DoAttack()
{
    // your existing damage code here...

    enemyAudio?.PlayAttack();
}

public class ZombieAI : MonoBehaviour
{
    public Transform player;          // Assigned dynamically
    public float speed = 3f;          // Movement speed
    public float stoppingDistance = 1f; // How close the zombie stops to player
    [Header("Attack Settings (Hitbox-based)")]
    [Tooltip("Reference to the child collider used as the attack hitbox (should be trigger)")]
    public Collider attackHitbox; // enable/disable via code or animation
    public float attackEnableDistance = 2.0f; // optional auto-enable when close

    private bool isDead = false;
    private HealthComponent health;
    // Hitbox drives damage; AI only handles movement and optional auto-enable

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        if (health != null) health.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        isDead = true;
    }

    void Update()
    {
        if (player == null || isDead) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Ignore vertical difference

    float dist = direction.magnitude;
    if (dist > stoppingDistance)
        {
            // Move towards player
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            // Rotate to face player
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  5f * Time.deltaTime);
        }
        else
        {
            // Optionally enable hitbox when in range; disable when out of range
            if (attackHitbox != null)
                attackHitbox.enabled = (dist <= attackEnableDistance);
        }
    }
}
<<<<<<< HEAD

=======
}
>>>>>>> 134f5411f1dd006e0e5a152834358817c03fb772
