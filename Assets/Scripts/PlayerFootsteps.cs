using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Clips")]
    public AudioClip dirtClip;
    public AudioClip woodClip;
    public AudioClip stoneClip;

    [Header("Settings")]
    public float minSpeed = 2f;   // minimum movement speed to trigger footsteps
    public float stepInterval = 0.45f;
    public LayerMask groundMask = ~0;

    private float stepTimer = 0f;
    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;

        if (speed < minSpeed)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            PlayFootstepSound();
            stepTimer = 0f;
        }
    }

    void PlayFootstepSound()
    {
        if (AudioManager.Instance == null) return;

        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f, groundMask))
        {
            string tag = hit.collider.tag;

            AudioClip clip = dirtClip; // default

            if (tag == "Stone") clip = stoneClip;
            else if (tag == "Wood") clip = woodClip;
            else if (tag == "Dirt") clip = dirtClip;

            AudioManager.Instance.PlaySFX(clip, 0.10f);
        }
    }
}
