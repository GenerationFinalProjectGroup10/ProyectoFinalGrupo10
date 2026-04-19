using UnityEngine;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        // Limpiar opciones
        qualityDropdown.ClearOptions();

        // Cargar nombres reales de calidad
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        // Cargar valor guardado
        int savedQuality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());

        qualityDropdown.value = savedQuality;
        qualityDropdown.RefreshShownValue();

        QualitySettings.SetQualityLevel(savedQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }
    public TMP_Dropdown resolutionDropdown;

    void SetupResolutions()
    {
        resolutionDropdown.ClearOptions();

        resolutionDropdown.AddOptions(new System.Collections.Generic.List<string>()
    {
        "800x600",
        "1280x720",
        "1920x1080"
    });
    }

    public void SetResolution(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(800, 600, false);
                break;
            case 1:
                Screen.SetResolution(1280, 720, false);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, false);
                break;
        }

        PlayerPrefs.SetInt("Resolution", index);
    }
}
