using UnityEngine;

public class escena2 : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.ambientSource.Stop();

        AudioManager.Instance.ambientSource.loop = false;
        AudioManager.Instance.ambientSource.volume = 0.10f;
        AudioManager.Instance.PlayAmbient(1);

        AudioManager.Instance.sfxSource.volume = 0.10f;
        AudioManager.Instance.PlaySFX(2);
    }
}