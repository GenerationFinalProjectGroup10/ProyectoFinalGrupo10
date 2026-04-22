using UnityEngine;

public class OpenObject : MonoBehaviour, IInteractable
{
    public ItemSO itemInside;
    public int amount = 1;

    [Header("Messages")]
    [SerializeField] private string interactMessage = "Presiona E para recoger";
    [SerializeField] private float pickupMessageDuration = 3f;

    private bool opened;
    private Collider objectCollider;
    private Renderer objectRenderer;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();
    }

    public string GetInteractMessage()
    {
        return opened ? "" : interactMessage;
    }

    public void Interact(PlayerController player)
    {
        if (opened || itemInside == null || InventoryManager.Instance == null) return;

        InventoryManager.Instance.inventory.AddItem(itemInside, amount);
        opened = true;

        string msg = string.IsNullOrWhiteSpace(itemInside.pickupMessage)
            ? "Recogiste " + itemInside.itemName
            : itemInside.pickupMessage;

        UI_Message.Instance?.Show(msg, false, pickupMessageDuration);

        if (objectCollider != null) objectCollider.enabled = false;
        if (objectRenderer != null) objectRenderer.enabled = false;
    }
}