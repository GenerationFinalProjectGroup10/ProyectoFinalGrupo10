using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class TemporizadorUI : MonoBehaviour
{
    public TextMeshProUGUI texto;
    private bool derrotaActivada = false;

    void Update()
    {
        if (ClockManager.Instance == null || derrotaActivada) return;

        var (h, m, s) = ClockManager.Instance.GetTimerTime();

        texto.text = $"{h:00}:{m:00}:{s:00}";

        // CONDICIÓN DE DERROTA
        if (h <= 0 && m <= 0 && s <= 0)
        {
            VerificarUbicacionJugador();
        }
    }

    //Verificación escena
    void VerificarUbicacionJugador()
    {
        string escenaActual = SceneManager.GetActiveScene().name;


        if (escenaActual == "Mundo2" || escenaActual == "Mundo3")
        {
            derrotaActivada = true; 
            SceneManager.LoadScene("Cinematica_Final_Oso");
        }
    }

    public void IniciarTimer()
    {
        ClockManager.Instance.TimerStart();
    }
}