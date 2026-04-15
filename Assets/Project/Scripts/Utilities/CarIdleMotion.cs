using UnityEngine;

public class CarIdleMotion : MonoBehaviour
{
    public float bounceAmount = 0.05f;
    public float bounceSpeed = 2f;

    public float tiltAmount = 2f;
    public float tiltSpeed = 2f;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation; // 🔥 guardamos rotación original (mirando en X)
    }

    void Update()
    {
        // 🔼 Rebote vertical
        float y = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localPosition = startPos + new Vector3(0, y, 0);

        // 🔄 Inclinación SUAVE sin perder dirección en X
        float tilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;

        transform.localRotation = startRot * Quaternion.Euler(0f, 0f, tilt);
    }
}