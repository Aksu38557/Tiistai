using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip music;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = music;
        audioSource.Play();
    }
}