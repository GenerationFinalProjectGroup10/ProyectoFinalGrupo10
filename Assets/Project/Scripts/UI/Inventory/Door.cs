using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO requiredKey;
    private Collider doorCollider;
    private bool opened;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }

    public string GetInteractMessage()
    {
        if (opened) return "";
        if (InventoryManager.Instance == null || requiredKey == null) return "";

        bool hasKey = InventoryManager.Instance.inventory.HasItem(requiredKey, 1);
        return hasKey ? "Presiona E para abrir la puerta" : "Necesitas una llave";
    }

    public void Interact(PlayerController player)
    {
        if (opened || InventoryManager.Instance == null || requiredKey == null) return;

        bool hasKey = InventoryManager.Instance.inventory.HasItem(requiredKey, 1);

        if (hasKey)
        {
            InventoryManager.Instance.inventory.RemoveItem(requiredKey, 1);
            OpenDoor();
        }
        else
        {
            UI_Message.Instance?.ShowTemporary("Necesitas una llave", 2f);
        }
    }

    private void OpenDoor()
    {
        opened = true;
        UI_Message.Instance?.ShowTemporary("¡Puerta abierta!", 2.5f);

        if (doorCollider != null) doorCollider.enabled = false;
    }
}