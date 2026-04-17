using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Listas de Audio")]
    public List<AudioClip> musicList;       // Ruta: Audio/Music
    public List<AudioClip> sfxList;         // Ruta: Audio/SFX
    public List<AudioClip> ambientList;     // Ruta: Audio/Ambient

    [Header("Objetos Padre (con varios sonidos)")]
    public List<GameObject> sfxParents;     // Objetos con hijos de tipo AudioSource
    public List<GameObject> ambientParents; // Igual que arriba pero para ambiente


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

    // --------------------------
    // MÉTODOS PRINCIPALES
    // --------------------------

    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicList.Count) return;

        musicSource.clip = musicList[index];
        musicSource.Play();
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxList.Count) return;

        sfxSource.PlayOneShot(sfxList[index]);
    }

    public void PlayAmbient(int index)
    {
        if (index < 0 || index >= ambientList.Count) return;

        ambientSource.clip = ambientList[index];
        ambientSource.Play();
    }

    // --------------------------
    // REPRODUCIR DESDE UN PADRE
    // --------------------------

    public void PlayRandomFromSFXParent(int parentIndex)
    {
        if (parentIndex < 0 || parentIndex >= sfxParents.Count) return;

        Transform parent = sfxParents[parentIndex].transform;

        int random = Random.Range(0, parent.childCount);
        AudioSource childAudio = parent.GetChild(random).GetComponent<AudioSource>();

        if (childAudio)
            sfxSource.PlayOneShot(childAudio.clip);
    }

    public void PlayRandomFromAmbientParent(int parentIndex)
    {
        if (parentIndex < 0 || parentIndex >= ambientParents.Count) return;

        Transform parent = ambientParents[parentIndex].transform;

        int random = Random.Range(0, parent.childCount);
        AudioSource childAudio = parent.GetChild(random).GetComponent<AudioSource>();

        if (childAudio)
            ambientSource.PlayOneShot(childAudio.clip);
    }
}