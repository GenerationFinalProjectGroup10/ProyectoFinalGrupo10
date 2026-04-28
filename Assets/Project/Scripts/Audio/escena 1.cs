using UnityEngine;

public class escena1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic(1);
        AudioManager.Instance.PlayAmbient(0);
    }

}
