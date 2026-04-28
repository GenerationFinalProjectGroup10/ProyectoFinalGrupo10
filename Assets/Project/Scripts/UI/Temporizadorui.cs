using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TemporizadorUI : MonoBehaviour
{
    public TextMeshProUGUI texto;
    private bool derrotaActivada = false;
    private bool victoriaActivada = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ClockManager.Instance == null) return;

        // 🆕 Marcar en el ClockManager (que persiste) cuando entra a zonas de juego real
        if (scene.name == "Mundo2" || scene.name == "Mundo3" || scene.name == "Sotano")
        {
            ClockManager.Instance.MarcarEntradaAlJuego();
        }

        // VICTORIA: solo si el ClockManager sabe que ya recorrió el juego
        if (scene.name == "Mundo1" && ClockManager.Instance.YaEntroAlJuego && !victoriaActivada && !derrotaActivada)
        {
            var (h, m, s) = ClockManager.Instance.GetTimerTime();
            bool hayTiempo = h > 0 || m > 0 || s > 0;

            if (hayTiempo)
            {
                victoriaActivada = true;
                ClockManager.Instance.DetenerReloj();
                StartCoroutine(MostrarVictoriaCoroutine());
            }
        }
    }

    private System.Collections.IEnumerator MostrarVictoriaCoroutine()
    {
        yield return null;
        VictoryUI victory = FindObjectOfType<VictoryUI>();
        if (victory != null)
            victory.Mostrar();
        else
            Debug.LogWarning("[TemporizadorUI] VictoryUI no encontrado en Mundo1.");
    }

    void Update()
    {
        if (ClockManager.Instance == null || derrotaActivada || victoriaActivada) return;

        var (h, m, s) = ClockManager.Instance.GetTimerTime();
        texto.text = $"{h:00}:{m:00}:{s:00}";

        if (h <= 0 && m <= 0 && s <= 0)
        {
            VerificarUbicacionJugador();
        }
    }

    void VerificarUbicacionJugador()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenaActual == "Mundo2" || escenaActual == "Mundo3" || escenaActual == "Sotano")
        {
            derrotaActivada = true;
            ClockManager.Instance.DetenerReloj();
            SceneManager.LoadScene("Cinematica_Final_Oso");
        }
    }

    public void IniciarTimer()
    {
        ClockManager.Instance.TimerStart();
    }
}