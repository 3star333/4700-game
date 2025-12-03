using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;     // for music if you add later
    public AudioSource sfxSource;       // guns, monsters, footsteps
    public AudioSource uiSource;        // UI clicks etc.
    public AudioSource ambienceSource;  // wind / background

    [Header("Ambience")]
    public AudioClip nightAmbience;
    public bool playAmbienceOnStart = true;

    [Header("UI Clips")]
    public AudioClip uiClick;
    public AudioClip uiHover;
    public AudioClip uiConfirm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (playAmbienceOnStart && nightAmbience != null && ambienceSource != null)
        {
            ambienceSource.loop = true;
            ambienceSource.clip = nightAmbience;
            ambienceSource.Play();
        }
    }

    // --------- SFX ---------
    public void PlaySFX(AudioClip clip, float pitchRandomize = 0.04f)
    {
        if (clip == null || sfxSource == null) return;

        float originalPitch = sfxSource.pitch;

        if (pitchRandomize > 0f)
        {
            sfxSource.pitch = Random.Range(1f - pitchRandomize, 1f + pitchRandomize);
        }

        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = originalPitch;
    }

    // --------- UI ---------
    public void PlayUISound(AudioClip clip)
    {
        if (clip == null || uiSource == null) return;
        uiSource.PlayOneShot(clip);
    }

    public void PlayUIClick()   => PlayUISound(uiClick);
    public void PlayUIHover()   => PlayUISound(uiHover);
    public void PlayUIConfirm() => PlayUISound(uiConfirm);
}
