using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryManager TEMPORAL de prueba.
/// Reemplazar por el sistema real cuando esté listo.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    // Singleton: una sola instancia accesible desde cualquier script
    public static InventoryManager Instance { get; private set; }

    // Lista interna que guarda los IDs de los items recogidos
    private List<string> _items = new List<string>();

    void Awake()
    {
        // Si ya existe una instancia, destruye este duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // No destruir este objeto al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Agrega un item al inventario por su ID.</summary>
    public void AddItem(string itemID)
    {
        _items.Add(itemID);
        Debug.Log($"[Inventario] Item agregado: {itemID} | Inventario actual: {string.Join(", ", _items)}");
    }

    /// <summary>Devuelve true si el inventario contiene ese item.</summary>
    public bool HasItem(string itemID)
    {
        return _items.Contains(itemID);
    }

    /// <summary>Elimina un item del inventario.</summary>
    public void RemoveItem(string itemID)
    {
        if (_items.Remove(itemID))
            Debug.Log($"[Inventario] Item eliminado: {itemID} | Inventario actual: {string.Join(", ", _items)}");
        else
            Debug.LogWarning($"[Inventario] Se intentó eliminar '{itemID}' pero no estaba en el inventario.");
    }
}