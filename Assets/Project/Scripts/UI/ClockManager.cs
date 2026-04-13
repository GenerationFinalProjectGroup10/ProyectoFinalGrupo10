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

    // ─── Eventos ──────────────────────────────────────────────────────────────
    public event Action<int> OnHourChime;
    public event Action<DateTime> OnMinuteTick;

    /// <summary>Se dispara cuando el temporizador llega a 0.</summary>
    public event Action OnTimerFinished;

    // ─── Estado interno reloj ─────────────────────────────────────────────────
    private DateTime _currentTime;
    private int _lastHourChecked = -1;
    private int _lastMinuteChecked = -1;
    private bool _isChiming = false;
    private bool _chimeTriggered = false;

    // ─── Temporizador regresivo ───────────────────────────────────────────────
    private const float MaxTimerSeconds = 24f * 3600f;  // 24 h de juego en segundos
    private float _timerRemainingSeconds = MaxTimerSeconds;
    private bool _timerRunning = false;
    private bool _timerFinished = false;

    /// <summary>Segundos restantes en el temporizador.</summary>
    public float TimerRemainingSeconds => _timerRemainingSeconds;

    /// <summary>¿Está corriendo el temporizador?</summary>
    public bool TimerIsRunning => _timerRunning;

    /// <summary>¿Ya llegó a cero?</summary>
    public bool TimerIsFinished => _timerFinished;

    // ─────────────────────────────────────────────────────────────────────────

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

    private void Start()
    {
        _currentTime = useRealTime ? DateTime.Now : DateTime.Today.AddHours(3);
        _lastHourChecked = _currentTime.Hour;
        _lastMinuteChecked = _currentTime.Minute;

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
        UpdateTimer();
    }

    // ─── Avanzar el tiempo del reloj ──────────────────────────────────────────

    private void AdvanceTime()
    {
        if (useRealTime)
            _currentTime = DateTime.Now;
        else
            _currentTime = _currentTime.AddSeconds(Time.deltaTime * gameTimeMultiplier);
    }

    // ─── Temporizador regresivo ───────────────────────────────────────────────

    private void UpdateTimer()
    {
        if (!_timerRunning || _timerFinished) return;

        // Descontar al mismo ritmo que el tiempo de juego
        float deltaGame = useRealTime ? Time.deltaTime : Time.deltaTime * gameTimeMultiplier;
        _timerRemainingSeconds -= deltaGame;

        if (_timerRemainingSeconds <= 0f)
        {
            _timerRemainingSeconds = 0f;
            _timerRunning = false;
            _timerFinished = true;
            Debug.Log("[ClockManager] ¡Temporizador llegó a 0!");
            OnTimerFinished?.Invoke();
        }
    }

    /// <summary>Inicia el temporizador (se llama automáticamente desde TimerUI).</summary>
    public void TimerStart()
    {
        if (!_timerFinished)
            _timerRunning = true;
    }

    /// <summary>Pausa el temporizador.</summary>
    public void TimerPause()
    {
        _timerRunning = false;
    }

    /// <summary>Reinicia el temporizador a 24 h de juego.</summary>
    public void TimerReset()
    {
        _timerRemainingSeconds = MaxTimerSeconds;
        _timerRunning = false;
        _timerFinished = false;
    }

    /// <summary>
    /// Devuelve el tiempo restante desglosado en horas, minutos y segundos de juego.
    /// </summary>
    public (int hours, int minutes, int seconds) GetTimerTime()
    {
        int total = Mathf.FloorToInt(_timerRemainingSeconds);
        int hours = total / 3600;
        int minutes = (total % 3600) / 60;
        int seconds = total % 60;
        return (hours, minutes, seconds);
    }

    // ─── Detectar cambio de hora ──────────────────────────────────────────────

    private void CheckForHourChange()
    {
        int currentHour = _currentTime.Hour;

        if (currentHour != _lastHourChecked)
        {
            _lastHourChecked = currentHour;
            _chimeTriggered = false;
        }

        if (!_chimeTriggered && !_isChiming)
        {
            _chimeTriggered = true;
            Debug.Log($"[ClockManager] ¡Nueva hora! {currentHour}:00");
            StartCoroutine(PlayChimes(1));
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

    // ─── Corrutina: tocar campanadas ──────────────────────────────────────────

    private System.Collections.IEnumerator PlayChimes(int count)
    {
        _isChiming = true;

        for (int i = 0; i < count; i++)
        {
            if (chimeClip != null)
                chimeAudioSource.PlayOneShot(chimeClip);
            else
                Debug.LogWarning("[ClockManager] No hay chimeClip asignado.");

            float waitTime = (chimeClip != null ? chimeClip.length : 0.5f) + timeBetweenChimes;
            yield return new WaitForSeconds(waitTime);
        }

        _isChiming = false;
    }

    // ─── Getters públicos ─────────────────────────────────────────────────────

    public DateTime GetCurrentTime() => _currentTime;

    public void ForceChime(int hour)
    {
        int chimesCount = hour % 12;
        if (chimesCount == 0) chimesCount = 12;
        if (!_isChiming)
            StartCoroutine(PlayChimes(chimesCount));
    }
}