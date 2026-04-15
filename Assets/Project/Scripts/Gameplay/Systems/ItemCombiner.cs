using UnityEngine;

public class ItemCombiner : MonoBehaviour
{
    [Header("IDs de los items necesarios para combinar")]
    public string idPieza1 = "reloj";
    public string idPieza2 = "manecilla";
    public string idResultado = "llave";

    [Header("Tecla para intentar combinar")]
    [Tooltip("El jugador presiona esta tecla cuando tiene ambas piezas en el inventario")]
    public KeyCode teclaCombinar = KeyCode.C;

    private bool _combinado = false;

    void Update()
    {
        // Si ya se combinó, no hacemos nada más
        if (_combinado) return;

        if (Input.GetKeyDown(teclaCombinar))
        {
            IntentarCombinar();
        }
    }

    public void IntentarCombinar()
    {
        bool tienePieza1 = InventoryManager.Instance.HasItem(idPieza1);
        bool tienePieza2 = InventoryManager.Instance.HasItem(idPieza2);

        if (tienePieza1 && tienePieza2)
        {
            // Quita las dos piezas del inventario
            InventoryManager.Instance.RemoveItem(idPieza1);
            InventoryManager.Instance.RemoveItem(idPieza2);

            // Agrega la llave al inventario
            InventoryManager.Instance.AddItem(idResultado);

            _combinado = true;
            Debug.Log("[ItemCombiner] ¡Combinación exitosa! Se obtuvo: " + idResultado);
        }
        else
        {
            // El jugador no tiene las dos piezas todavía
            Debug.Log("[ItemCombiner] Faltan piezas para combinar.");
            // Aquí puedes mostrar un mensaje en pantalla al jugador si quieres
        }
    }
}