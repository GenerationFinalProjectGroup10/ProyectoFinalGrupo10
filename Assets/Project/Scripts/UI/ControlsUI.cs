using UnityEngine;

public class ControlsUI : MonoBehaviour
{
    public GameObject controlsPanel;

    private bool panelVisible = true;

    void Start()
    {
        // Mostramos el panel al iniciar
        controlsPanel.SetActive(true);

        // Pausamos el juego mientras el jugador lee los controles
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Cualquier tecla cierra el panel y reanuda el juego
        if (panelVisible && Input.anyKeyDown)
        {
            controlsPanel.SetActive(false);
            panelVisible = false;
            Time.timeScale = 1f;
        }
    }
}