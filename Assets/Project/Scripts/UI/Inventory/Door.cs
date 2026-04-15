using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public bool requiresKey = true;
    public Item.ItemType requiredKey = Item.ItemType.Key;

    private bool isOpen = false;

    public void Interact(PlayerController player)
    {
        if (isOpen) return;

        Inventory inventory = InventoryManager.Instance.inventory;

        Debug.Log("Intentando consumir llave...");

        //Inventory inventory = player.GetInventory();

        if (requiresKey)
        {
            if (inventory.HasItem(requiredKey))
            {
                inventory.RemoveItem(requiredKey); // 🔥 no if

                Debug.Log("Llave consumida");
                OpenDoor();
            }
            else
            {
                Debug.Log("Necesitas una llave");
            }
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        transform.Rotate(0, 90f, 0);
        Debug.Log("Puerta abierta");
    }
}