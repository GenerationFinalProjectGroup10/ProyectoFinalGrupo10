using UnityEngine;

public class TestIniciarReloj : MonoBehaviour
{
    public KeyCode tecla = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(tecla))
        {
            if (ClockManager.Instance != null)
            {
                ClockManager.Instance.IniciarReloj();
                Debug.Log("🧪 Reloj iniciado manualmente");
            }
            else
            {
                Debug.LogWarning("No existe ClockManager en la escena");
            }
        }
    }
}