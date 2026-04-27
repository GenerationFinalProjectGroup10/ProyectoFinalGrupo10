using UnityEngine;

public class PaperInteraction : MonoBehaviour
{
    [Header("Configuración de Detección")]
    [Tooltip("Distancia a la que el jugador puede leer el papel")]
    public float interactionDistance = 2.5f;

    [Header("Referencia al UI Manager")]
    public PaperUIManager uiManager;

    private Transform playerTransform;
    private bool isPlayerNearby = false;
    private bool isNoteOpen = false;

    void Start()
    {
        // Buscar al jugador automáticamente por su tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("PaperInteraction: No se encontró un GameObject con tag 'Player'.");
        }

        // Verificar que el uiManager esté asignado
        if (uiManager == null)
        {
            Debug.LogError("PaperInteraction: El campo 'UI Manager' no está asignado en el Inspector.");
        }
    }

    void Update()
    {
        if (playerTransform == null || uiManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Detectar si el jugador está cerca
        if (distance <= interactionDistance)
        {
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
                uiManager.ShowPrompt(true); // Mostrar "Presiona E para leer"
            }

            // Si el jugador presiona E
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isNoteOpen)
                {
                    isNoteOpen = true;
                    uiManager.ShowPrompt(false);   // Ocultar el prompt
                    uiManager.ShowNote(true);       // Mostrar la nota
                    // Opcional: pausar el juego mientras lee
                    // Time.timeScale = 0f;
                }
                else
                {
                    // Si ya está abierta, cerrar con E también
                    CloseNote();
                }
            }
        }
        else
        {
            // El jugador se alejó
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                uiManager.ShowPrompt(false);

                // Si la nota estaba abierta, cerrarla automáticamente
                if (isNoteOpen)
                {
                    CloseNote();
                }
            }
        }
    }

    // Método público para que el botón "Cerrar" del Canvas también lo llame
    public void CloseNote()
    {
        isNoteOpen = false;
        uiManager.ShowNote(false);
        // Si habías pausado: Time.timeScale = 1f;
    }
}