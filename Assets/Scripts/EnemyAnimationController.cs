using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Example: check if the enemy is moving
        if (GetComponent<Rigidbody>().linearVelocity.magnitude > 0.001f)
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }
}
