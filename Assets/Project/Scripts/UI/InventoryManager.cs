using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Configuración")]
    public int maxSlots = 20;

    // Lista interna de ítems
    private List<ItemData> items = new List<ItemData>();

    // Eventos para que la UI reaccione automáticamente
    [HideInInspector] public UnityEvent OnInventoryChanged = new UnityEvent();

    void Awake()
    {
        // Patrón Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Agregar ítem
    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno.");
            return false;
        }

        items.Add(item);
        OnInventoryChanged.Invoke();
        Debug.Log($"[Inventario] Recogido: {item.itemName}");
        return true;
    }

    //Remover ítem
    public bool RemoveItem(ItemData item)
    {
        if (!items.Contains(item)) return false;
        items.Remove(item);
        OnInventoryChanged.Invoke();
        return true;
    }

    //Usar ítem
    // El ítem debe implementar IUsable para que esto funcione
    public void UseItem(ItemData item, GameObject user)
    {
        if (item is IUsable usable)
        {
            usable.Use(user);
            // Los ítems de un solo uso se eliminan automáticamente
            if (item.type != ItemType.Key)
                RemoveItem(item);
        }
        else
        {
            Debug.Log($"{item.itemName} no tiene uso definido.");
        }
    }

    //Verificar si tienes un ítem por ID
    public bool HasItem(string itemID)
    {
        return items.Exists(i => i.itemID == itemID);
    }

    public ItemData GetItem(string itemID)
    {
        return items.Find(i => i.itemID == itemID);
    }

    public List<ItemData> GetAllItems() => new List<ItemData>(items);
}