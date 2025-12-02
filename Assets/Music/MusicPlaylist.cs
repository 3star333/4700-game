using UnityEngine;

public class MusicPlaylist : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] songs;
    private int currentSong = 0;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        PlaySong(currentSong);
    }

    void Update()
    {
        // If the song finished playing, go to the next one
        if (!audioSource.isPlaying)
        {
            currentSong = (currentSong + 1) % songs.Length; // loops back to 0
            PlaySong(currentSong);
        }
    }

    void PlaySong(int index)
    {
        audioSource.clip = songs[index];
        audioSource.Play();
    }
}
