using UnityEngine;

public class LampController : MonoBehaviour
{
    public Light lampLight;

    [Range(0, 10)]
    public float intensity = 2f;

    [Range(0, 100)]
    public float range = 20f;

    [Range(0, 180)]
    public float spotAngle = 45f;

    [Range(1000, 10000)]
    public float temperature = 3000f;

    public Color lightColor = Color.white;

    void Update()
    {
        lampLight.intensity = intensity;
        lampLight.range = range;
        lampLight.spotAngle = spotAngle;
        lampLight.color = lightColor;
        lampLight.colorTemperature = temperature;
    }
}