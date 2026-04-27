using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClockManager : MonoBehaviour
{
    public static ClockManager Instance { get; private set; }

    [Header("Configuración del Reloj")]
    public float gameTimeMultiplier = 60f;

    [Header("Audio")]
    public AudioSource chimeAudioSource;
    public AudioClip chimeClip;

    public event Action<int> OnHourChime;
    public event Action<DateTime> OnMinuteTick;
    public event Action OnTimerFinished;

    private DateTime _currentTime;
    private int _lastHourChecked;
    private int _lastMinuteChecked;

    private bool _isChiming = false;
    private bool _relojActivo = false;

    private const float MaxTimerSeconds = 24f * 3600f;
    private float _timerRemainingSeconds = MaxTimerSeconds;
    private bool _timerRunning = false;
    private bool _timerFinished = false;

    public float TimerRemainingSeconds => _timerRemainingSeconds;

    void Awake()
    {
        Debug.Log("🕰️ Awake ClockManager: " + gameObject.GetInstanceID());

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🔥 Inicializar aquí, no en Start
        _currentTime = DateTime.Today.AddHours(3);
        _lastHourChecked = _currentTime.Hour;
        _lastMinuteChecked = _currentTime.Minute;

        // ✅ Arranque automático: el reloj corre desde el primer frame
        IniciarReloj();
    }

    void Start()
    {
        // Suscribirse con un frame de delay para no contar la escena inicial como "cambio de escena"
        StartCoroutine(RegistrarEventoCambioEscena());
    }

    private IEnumerator RegistrarEventoCambioEscena()
    {
        yield return null; // espera un frame
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (!_relojActivo) return;

        AdvanceTime();
        CheckHourChange();
        CheckMinuteChange();
        UpdateTimer();
    }

    // 🟢 ACTIVAR RELOJ
    public void IniciarReloj()
    {
        _relojActivo = true;
        _timerRunning = true;
        Debug.Log("Reloj iniciado a las 3 AM");
    }

    // ⏩ CAMBIO DE ESCENA
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_relojActivo) return;

        AdelantarUnaHora();
    }

    public void AdelantarUnaHora()
    {
        _currentTime = _currentTime.AddHours(1);
        _timerRemainingSeconds -= 3600f;

        if (_timerRemainingSeconds < 0)
            _timerRemainingSeconds = 0;

        Debug.Log("Cambio de escena: +1 hora");
    }

    private void AdvanceTime()
    {
        _currentTime = _currentTime.AddSeconds(Time.deltaTime * gameTimeMultiplier);
    }

    private void UpdateTimer()
    {
        if (!_timerRunning || _timerFinished) return;

        _timerRemainingSeconds -= Time.deltaTime * gameTimeMultiplier;

        if (_timerRemainingSeconds <= 0)
        {
            _timerRemainingSeconds = 0;
            _timerRunning = false;
            _timerFinished = true;
            OnTimerFinished?.Invoke();
        }
    }

    public (int h, int m, int s) GetTimerTime()
    {
        int total = Mathf.FloorToInt(_timerRemainingSeconds);
        return (total / 3600, (total % 3600) / 60, total % 60);
    }

    private void CheckHourChange()
    {
        if (_currentTime.Hour != _lastHourChecked)
        {
            _lastHourChecked = _currentTime.Hour;
            OnHourChime?.Invoke(_lastHourChecked);
            ForceChime(1);
        }
    }

    private void CheckMinuteChange()
    {
        if (_currentTime.Minute != _lastMinuteChecked)
        {
            _lastMinuteChecked = _currentTime.Minute;
            OnMinuteTick?.Invoke(_currentTime);
        }
    }

    IEnumerator PlayChimes(int count)
    {
        _isChiming = true;

        for (int i = 0; i < count; i++)
        {
            if (chimeClip != null)
                chimeAudioSource.PlayOneShot(chimeClip);

            yield return new WaitForSeconds(1f);
        }

        _isChiming = false;
    }

    // 🔊 FIX ERROR 1
    public void ForceChime(int cantidad = 1)
    {
        if (!_isChiming)
            StartCoroutine(PlayChimes(cantidad));
    }

    // ▶️ FIX ERROR 2
    public void TimerStart()
    {
        IniciarReloj();
    }

    public DateTime GetCurrentTime() => _currentTime;

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}