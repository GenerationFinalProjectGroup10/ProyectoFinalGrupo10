using UnityEngine;
using TMPro;

public class TemporizadorUI : MonoBehaviour
{
    public TextMeshProUGUI texto;

    void Update()
    {
        if (ClockManager.Instance == null) return;

        var (h, m, s) = ClockManager.Instance.GetTimerTime();

        Debug.Log("UI TIME: " + h + ":" + m + ":" + s);

        texto.text = $"{h:00}:{m:00}:{s:00}";
    }

    public void IniciarTimer()
    {
        ClockManager.Instance.TimerStart();
    }
}