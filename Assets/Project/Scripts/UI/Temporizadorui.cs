using UnityEngine;
using TMPro;

/// <summary>
/// Muestra el temporizador regresivo de 24 horas de juego (24:00:00 → 00:00:00).
/// Se inicia automáticamente al comenzar el juego.
/// Adjunta este script al GameObject "Cronometro" en el Canvas.
/// </summary>
public class TemporizadorUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Texto donde se muestra el tiempo HH:MM:SS.")]
    public TMP_Text timerText;

    [Header("Formato")]
    [Tooltip("Mostrar segundos en el temporizador.")]
    public bool showSeconds = true;

    [Header("Color final")]
    [Tooltip("Color del texto cuando queda menos de este tiempo (segundos de juego). 0 = desactivado.")]
    public float warningThresholdSeconds = 3600f;   // 1 hora de juego por defecto

    [Tooltip("Color normal del texto.")]
    public Color normalColor = Color.white;

    [Tooltip("Color de advertencia cuando el tiempo es bajo.")]
    public Color warningColor = Color.red;

    private ClockManager _clockManager;

    private void Start()
    {
        _clockManager = ClockManager.Instance;

        if (_clockManager == null)
        {
            Debug.LogError("[TemporizadorUI] No se encontró ClockManager en la escena.");
            enabled = false;
            return;
        }

        // Suscribirse al evento de fin de tiempo
        _clockManager.OnTimerFinished += HandleTimerFinished;

        // Iniciar automáticamente al comenzar el juego
        _clockManager.TimerStart();

        UpdateDisplay();
    }

    private void OnDestroy()
    {
        if (_clockManager != null)
            _clockManager.OnTimerFinished -= HandleTimerFinished;
    }

    private void Update()
    {
        if (_clockManager == null) return;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        var (h, m, s) = _clockManager.GetTimerTime();

        if (timerText != null)
        {
            timerText.text = showSeconds
                ? $"{h:D2}:{m:D2}:{s:D2}"
                : $"{h:D2}:{m:D2}";

            // Cambiar color si queda poco tiempo
            if (warningThresholdSeconds > 0f)
            {
                timerText.color = _clockManager.TimerRemainingSeconds <= warningThresholdSeconds
                    ? warningColor
                    : normalColor;
            }
        }
    }

    private void HandleTimerFinished()
    {
        Debug.Log("[TemporizadorUI] ¡El tiempo se acabó!");

        if (timerText != null)
        {
            timerText.text = "00:00:00";
            timerText.color = warningColor;
        }

        // Aquí puedes agregar lógica adicional: mostrar pantalla de fin, etc.
    }
}