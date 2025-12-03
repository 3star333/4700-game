using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip idleClip;
    public AudioClip attackClip;
    public AudioClip deathClip;

    [Header("Idle Settings")]
    public float idleMinDelay = 3f;
    public float idleMaxDelay = 7f;

    private AudioSource localSource;
    private Coroutine idleRoutine;

    private void Awake()
    {
        localSource = GetComponent<AudioSource>();
        localSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        StartIdle();
    }

    private void OnDisable()
    {
        if (idleRoutine != null)
            StopCoroutine(idleRoutine);
    }

    public void StartIdle()
    {
        if (idleRoutine != null)
            StopCoroutine(idleRoutine);

        idleRoutine = StartCoroutine(IdleLoop());
    }

    private IEnumerator IdleLoop()
    {
        while (true)
        {
            float wait = Random.Range(idleMinDelay, idleMaxDelay);
            yield return new WaitForSeconds(wait);

            if (idleClip != null)
                localSource.PlayOneShot(idleClip);
        }
    }

    public void PlayAttack()
    {
        if (attackClip != null)
            localSource.PlayOneShot(attackClip);
    }

    public void PlayDeath()
    {
        if (deathClip != null)
            localSource.PlayOneShot(deathClip);
    }
}
