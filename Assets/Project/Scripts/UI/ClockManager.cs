using System;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────────────
    public static ClockManager Instance { get; private set; }

    // ─── Configuración exportada al Inspector ─────────────────────────────────
    [Header("Configuración del Reloj")]
    [Tooltip("Si es true, usa la hora real del sistema. Si es false, usa hora del juego.")]
    public bool useRealTime = true;

    [Tooltip("Velocidad del tiempo del juego (solo si useRealTime = false). Ej: 60 = 1 min de juego por segundo real.")]
    public float gameTimeMultiplier = 60f;

    [Header("Audio")]
    [Tooltip("AudioSource que reproducirá las campanadas.")]
    public AudioSource chimeAudioSource;

    [Tooltip("Clip de una sola campanada.")]
    public AudioClip chimeClip;

    [Tooltip("Segundos entre cada campanada cuando son varias horas.")]
    public float timeBetweenChimes = 1.2f;

    //[Header("Eventos")]
    // Otros scripts pueden suscribirse a este evento
    public event Action<int> OnHourChime;   // envía la hora que sonó
    public event Action<DateTime> OnMinuteTick; // cada minuto

    // ─── Estado interno ───────────────────────────────────────────────────────
    private DateTime _currentTime;
    private int _lastHourChecked = -1;
    private int _lastMinuteChecked = -1;
    private bool _isChiming = false;
    private bool _chimeTriggered = false;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // Patrón Singleton: solo existirá una instancia de ClockManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre escenas
    }

    private void Start()
    {
        // Inicializa la hora
        _currentTime = useRealTime ? DateTime.Now : DateTime.Today.AddHours(3); // juego empieza a las 8am
        _lastHourChecked = _currentTime.Hour;
        _lastMinuteChecked = _currentTime.Minute;

        // Valida que el AudioSource exista
        if (chimeAudioSource == null)
        {
            chimeAudioSource = gameObject.AddComponent<AudioSource>();
            chimeAudioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        AdvanceTime();
        CheckForHourChange();
        CheckForMinuteChange();
    }

    // ─── Avanzar el tiempo ────────────────────────────────────────────────────

    private void AdvanceTime()
    {
        if (useRealTime)
        {
            _currentTime = DateTime.Now;
        }
        else
        {
            // Tiempo del juego: multiplica deltaTime por el multiplicador
            _currentTime = _currentTime.AddSeconds(Time.deltaTime * gameTimeMultiplier);
        }
    }

    // ─── Detectar cambio de hora ──────────────────────────────────────────────

    private void CheckForHourChange()
    {
        int currentHour = _currentTime.Hour;

        if (currentHour != _lastHourChecked)
        {
            _lastHourChecked = currentHour;
            _chimeTriggered = false; // hora nueva, resetear bandera
        }

        // Solo ejecuta si no ha sonado aún en esta hora y no está sonando
        if (!_chimeTriggered && !_isChiming)
        {
            _chimeTriggered = true; // bloquear futuros disparos de esta hora

            int chimesCount = 1;

            Debug.Log($"[ClockManager] ¡Nueva hora! {currentHour}:00 → {chimesCount} campanada(s)");

            StartCoroutine(PlayChimes(chimesCount));
            OnHourChime?.Invoke(currentHour);
        }
    }

    private void CheckForMinuteChange()
    {
        int currentMinute = _currentTime.Minute;

        if (currentMinute != _lastMinuteChecked)
        {
            _lastMinuteChecked = currentMinute;
            OnMinuteTick?.Invoke(_currentTime);
        }
    }

    // ─── Corrutina: tocar N campanadas ────────────────────────────────────────

    private System.Collections.IEnumerator PlayChimes(int count)
    {
        _isChiming = true;

        for (int i = 0; i < count; i++)
        {
            if (chimeClip != null)
            {
                chimeAudioSource.PlayOneShot(chimeClip);
            }
            else
            {
                Debug.LogWarning("[ClockManager] No hay chimeClip asignado.");
            }

            // Esperar la duración de la campanada + pausa entre campanadas
            float waitTime = (chimeClip != null ? chimeClip.length : 0.5f) + timeBetweenChimes;
            yield return new WaitForSeconds(waitTime);
        }

        _isChiming = false;
    }

    // ─── Getters públicos ─────────────────────────────────────────────────────

    /// <summary>Retorna la hora actual del reloj.</summary>
    public DateTime GetCurrentTime() => _currentTime;

    /// <summary>Fuerza que el reloj toque las campanadas de una hora específica. Útil para pruebas.</summary>
    public void ForceChime(int hour)
    {
        int chimesCount = hour % 12;
        if (chimesCount == 0) chimesCount = 12;
        if (!_isChiming)
            StartCoroutine(PlayChimes(chimesCount));
    }
}