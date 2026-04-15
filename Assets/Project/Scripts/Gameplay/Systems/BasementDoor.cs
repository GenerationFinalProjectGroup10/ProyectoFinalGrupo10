using UnityEngine;
using UnityEngine.SceneManagement;

public class BasementDoor : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre exacto de la escena del sótano (como aparece en Build Settings)")]
    public string nombreEscenaSotano = "Sotano";

    [Tooltip("ID del item que se necesita para abrir la puerta")]
    public string idLlave = "llave";

    [Tooltip("Distancia máxima desde la que el jugador puede interactuar con la puerta")]
    public float rangoInteraccion = 2f;

    private Transform _player;
    private bool _playerNearby = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    void Update()
    {
        if (_player == null) return;

        float distancia = Vector3.Distance(transform.position, _player.position);
        _playerNearby = distancia <= rangoInteraccion;

        if (_playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            IntentarAbrir();
        }
    }

    void IntentarAbrir()
    {
        if (InventoryManager.Instance.HasItem(idLlave))
        {
            Debug.Log("[BasementDoor] Puerta abierta. Cargando escena: " + nombreEscenaSotano);

            // Consume la llave (opcional, quítalo si quieres que el jugador la conserve)
            InventoryManager.Instance.RemoveItem(idLlave);

            // Cambia de escena
            SceneManager.LoadScene(nombreEscenaSotano);
        }
        else
        {
            Debug.Log("[BasementDoor] Necesitas una llave para abrir esta puerta.");
            // Puedes mostrar un texto en pantalla aquí
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoInteraccion);
    }
}