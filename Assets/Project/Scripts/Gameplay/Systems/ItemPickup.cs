using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Configuración del item")]
    [Tooltip("ID único del item. Usa: 'reloj' o 'manecilla'")]
    public string itemID;

    [Tooltip("Distancia máxima desde la que el jugador puede recoger el objeto")]
    public float pickupRange = 2.5f;

    [Tooltip("Texto que aparece en pantalla para indicar la acción")]
    public string promptText = "Presiona E para recoger";

    private Transform _player;
    private bool _playerNearby = false;

    void Start()
    {
        // Buscamos el jugador por su tag. Asegúrate de que tu jugador tenga el tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    void Update()
    {
        if (_player == null) return;

        float distancia = Vector3.Distance(transform.position, _player.position);
        _playerNearby = distancia <= pickupRange;

        // Si el jugador está cerca y presiona E, recoge el objeto
        if (_playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            RecogerItem();
        }
    }

    void RecogerItem()
    {
        // Le decimos al inventario que agregue este item
        InventoryManager.Instance.AddItem(itemID);

        Debug.Log($"[ItemPickup] Item recogido: {itemID}");

        // Desactivamos el objeto del mundo (ya está en el inventario)
        gameObject.SetActive(false);
    }

    // Dibuja el rango de pickup en el editor para que puedas verlo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}