using UnityEngine;

public class OpenObject : MonoBehaviour, IInteractable
{
    public ItemSO itemInside;
    public int amount = 1;

    [SerializeField] private string interactMessage = "Presiona E para recoger";
    private bool opened;

    private Collider objectCollider;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
    }

    public string GetInteractMessage() => opened ? "" : interactMessage;

    public void Interact(PlayerController player)
    {
        if (opened || itemInside == null || InventoryManager.Instance == null) return;

        InventoryManager.Instance.inventory.AddItem(itemInside, amount);
        opened = true;

        UI_Message.Instance?.Show("Recogiste " + itemInside.itemName);
        gameObject.SetActive(false); //Desactiva el objeto para simular que se ha recogido
    }
}