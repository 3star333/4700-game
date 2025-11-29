using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public Transform player;          // Assigned dynamically
    public float speed = 3f;          // Movement speed
    public float stoppingDistance = 1f; // How close the zombie stops to player

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Ignore vertical difference

        if (direction.magnitude > stoppingDistance)
        {
            // Move towards player
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            // Rotate to face player
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  5f * Time.deltaTime);
        }
    }
}
