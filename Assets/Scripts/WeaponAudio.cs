using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    [Header("Weapon Audio Clips")]
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public AudioClip dryFireClip;

    public void PlayShoot()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(shootClip);
    }

    public void PlayReload()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(reloadClip, 0f);
    }

    public void PlayDryFire()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(dryFireClip, 0f);
    }
}
