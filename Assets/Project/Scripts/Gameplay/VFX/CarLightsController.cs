using UnityEngine;

public class CarLightsController : MonoBehaviour
{
    public Light leftLight;
    public Light rightLight;

    [Header("Configuración Global")]
    [Range(0, 10)] public float intensity = 2f;
    [Range(0, 100)] public float range = 20f;
    [Range(0, 180)] public float spotAngle = 45f;
    [Range(1000, 10000)] public float temperature = 3000f;
    public Color color = Color.white;

    void Update()
    {
        ApplySettings(leftLight);
        ApplySettings(rightLight);
    }

    void ApplySettings(Light light)
    {
        if (light == null) return;

        light.intensity = intensity;
        light.range = range;
        light.spotAngle = spotAngle;
        light.colorTemperature = temperature;
        light.color = color;
    }
}