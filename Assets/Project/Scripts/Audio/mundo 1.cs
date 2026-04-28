using UnityEngine;

public class mundo1 : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.musicSource.volume = 0.25f;
        AudioManager.Instance.PlayMusic(1);
    }
}