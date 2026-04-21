using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        // Cargar volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        slider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // Escuchar cambios del slider
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        Debug.Log("Volumen cambiado a: " + value);

        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }
}