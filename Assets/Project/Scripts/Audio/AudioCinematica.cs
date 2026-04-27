using UnityEngine;

public class AudioCinematica : MonoBehaviour
{
    private AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
    }

    public void PlayVoz() {
        source.Play();
    }
}