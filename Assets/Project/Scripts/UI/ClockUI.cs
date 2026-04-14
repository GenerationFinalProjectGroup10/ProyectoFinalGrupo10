using System;
using UnityEngine;
using TMPro;                  // TextMeshPro
using System.Collections;

public class ClockUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Texto principal donde se muestra la hora.")]
    public TMP_Text timeText;

    [Tooltip("Texto secundario donde se muestra la fecha.")]
    public TMP_Text dateText;

    [Tooltip("Imagen o panel que parpadea cuando suena la campanada (opcional).")]
    public UnityEngine.UI.Image chimeIndicator;

    [Header("Formato")]
    [Tooltip("true = formato 12h con AM/PM | false = formato 24h")]
    public bool use12HourFormat = false;

    [Tooltip("Mostrar segundos en el reloj.")]
    public bool showSeconds = true;

    [Header("Animación de campanada")]
    [Tooltip("Color normal del indicador.")]
    public Color normalColor = new Color(1f, 1f, 1f, 0f);      // transparente

    [Tooltip("Color cuando suena la campanada.")]
    public Color chimeColor = new Color(1f, 0.9f, 0.2f, 1f);   // amarillo brillante

    [Tooltip("Duración del parpadeo en segundos.")]
    public float chimePulseDuration = 0.4f;

    // ─── Variables internas ───────────────────────────────────────────────────
    private ClockManager _clockManager;

    // ─────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        // Busca el ClockManager (puede estar en otro GameObject)
        _clockManager = ClockManager.Instance;

        if (_clockManager == null)
        {
            Debug.LogError("[ClockUI] No se encontró ClockManager en la escena.");
            return;
        }

        // Suscribirse al evento de campanada para animar el indicador
        _clockManager.OnHourChime += HandleChimeAnimation;

        // Inicializar colores
        if (chimeIndicator != null)
            chimeIndicator.color = normalColor;

        // Actualizar inmediatamente para no mostrar valores vacíos al inicio
        UpdateDisplay(_clockManager.GetCurrentTime());
    }

    private void OnDestroy()
    {
        // Siempre desuscribirse para evitar memory leaks
        if (_clockManager != null)
            _clockManager.OnHourChime -= HandleChimeAnimation;
    }

    private void Update()
    {
        if (_clockManager == null) return;
        UpdateDisplay(_clockManager.GetCurrentTime());
    }

    // ─── Actualizar textos ────────────────────────────────────────────────────

    private void UpdateDisplay(DateTime time)
    {
        if (timeText != null)
            timeText.text = FormatTime(time);

        if (dateText != null)
            dateText.text = FormatDate(time);
    }

    private string FormatTime(DateTime time)
    {
        if (use12HourFormat)
        {
            string ampm = time.Hour >= 12 ? "PM" : "AM";
            int hour12 = time.Hour % 12;
            if (hour12 == 0) hour12 = 12;

            return showSeconds
                ? $"{hour12:D2}:{time.Minute:D2}:{time.Second:D2} {ampm}"
                : $"{hour12:D2}:{time.Minute:D2} {ampm}";
        }
        else
        {
            return showSeconds
                ? $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}"
                : $"{time.Hour:D2}:{time.Minute:D2}";
        }
    }

    private string FormatDate(DateTime time)
    {
        // Ejemplo: "Lunes, 12 de Abril de 2026"
        string[] days = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
        string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        return $"{days[(int)time.DayOfWeek]}, {time.Day} de {months[time.Month - 1]} de {time.Year}";
    }

    // ─── Animación al sonar ───────────────────────────────────────────────────

    private void HandleChimeAnimation(int hour)
    {
        if (chimeIndicator != null)
            StartCoroutine(PulseIndicator());
    }

    private IEnumerator PulseIndicator()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < chimePulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (chimePulseDuration / 2f);
            if (chimeIndicator != null)
                chimeIndicator.color = Color.Lerp(normalColor, chimeColor, t);
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < chimePulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (chimePulseDuration / 2f);
            if (chimeIndicator != null)
                chimeIndicator.color = Color.Lerp(chimeColor, normalColor, t);
            yield return null;
        }

        if (chimeIndicator != null)
            chimeIndicator.color = normalColor;
    }
}