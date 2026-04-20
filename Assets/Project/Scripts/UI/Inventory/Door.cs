using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO requiredKey;
    private Collider doorCollider;
    
    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }

    public string GetInteractMessage()
    {
        if (InventoryManager.Instance == null || requiredKey == null) return "";

        bool hasKey = InventoryManager.Instance.inventory.HasItem(requiredKey, 1);
        return hasKey ? "Presiona E para abrir la puerta" : "Necesitas una llave";
    }

    public void Interact(PlayerController player)
    {
        if (InventoryManager.Instance == null || requiredKey == null) return;

        bool hasKey = InventoryManager.Instance.inventory.HasItem(requiredKey, 1);

        if (hasKey)
        {
            InventoryManager.Instance.inventory.RemoveItem(requiredKey, 1);
            OpenDoor();
        }
        else
        {
            UI_Message.Instance?.Show("Necesitas una llave");
        }
    }

    private void OpenDoor()
    {
        UI_Message.Instance?.Show("¡Puerta abierta!");

        if (doorCollider != null) doorCollider.enabled = false; //Desactiva collider
    }
}