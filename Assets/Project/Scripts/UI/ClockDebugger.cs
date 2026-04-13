using UnityEngine;

/// <summary>
/// Script de prueba. Adjúntalo a cualquier GameObject para probar el reloj
/// sin esperar a que realmente sean las 12:00.
/// </summary>
public class ClockDebugger : MonoBehaviour
{
    [Header("Prueba rápida")]
    [Tooltip("Presiona esta tecla para forzar campanadas de prueba.")]
    public KeyCode testKey = KeyCode.Space;

    [Tooltip("Hora que se simulará al presionar la tecla (1-12).")]
    [Range(1, 12)]
    public int testHour = 3;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            if (ClockManager.Instance != null)
            {
                Debug.Log($"[ClockDebugger] Forzando {testHour} campanada(s)...");
                ClockManager.Instance.ForceChime(testHour);
            }
        }
    }
}