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
    private float openTime = 0f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("PaperInteraction: No se encontró un GameObject con tag 'Player'.");
        }

        if (uiManager == null)
        {
            Debug.LogError("PaperInteraction: El campo 'UI Manager' no está asignado en el Inspector.");
        }
    }

    void Update()
    {
        if (playerTransform == null || uiManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            // Mostrar prompt solo si la nota no está abierta
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
                if (!isNoteOpen)
                    uiManager.ShowPrompt(true);
            }

            // Abrir nota con E
            if (Input.GetKeyDown(KeyCode.E) && !isNoteOpen)
            {
                isNoteOpen = true;
                openTime = Time.time;
                uiManager.ShowPrompt(false);
                uiManager.ShowNote(true);
            }

            // Cerrar con cualquier tecla, esperando 0.2s para evitar conflicto con E
            if (isNoteOpen && Input.anyKeyDown && Time.time > openTime + 0.2f)
            {
                CloseNote();
            }
        }
        else
        {
            // El jugador se alejó
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                uiManager.ShowPrompt(false);

                if (isNoteOpen)
                {
                    CloseNote();
                }
            }
        }
    }

    public void CloseNote()
    {
        isNoteOpen = false;
        uiManager.ShowNote(false);
    }
}