using UnityEngine;

public class OpenObject : MonoBehaviour, IInteractable
{
    public ItemSO itemInside;
    public int amount = 1;

    [Header("Messages")]
    [SerializeField] private string interactMessage = "Presiona E para recoger";
    [SerializeField] private string combineMessage = "Presiona C para juntar la pieza del reloj";
    [SerializeField] private float pickupMessageDuration = 3f;

    [Header("Special Flags")]
    [SerializeField] private bool isSecondClockPiece;

    private bool opened;
    private Collider objectCollider;
    private Renderer objectRenderer;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();
    }

    public bool IsSecondClockPiece() => isSecondClockPiece;

    public string GetInteractMessage()
    {
        if (opened) return "";
        if (isSecondClockPiece) return combineMessage;
        if (itemInside == null) return "";
        return interactMessage;
    }

    public void Interact(PlayerController player)
    {
        if (opened || itemInside == null || InventoryManager.Instance == null || isSecondClockPiece) return;

        var inventory = InventoryManager.Instance.inventory;
        if (inventory == null) return;

        inventory.AddItem(itemInside, amount);
        opened = true;

        string msg = string.IsNullOrWhiteSpace(itemInside.pickupMessage)
            ? "Recogiste " + itemInside.itemName
            : itemInside.pickupMessage;

        UI_Message.Instance?.ShowTemporary(msg, pickupMessageDuration);

        // Solo desactiva colider y renderer si NO es la segunda pieza del reloj
        if (!isSecondClockPiece)
        {
            if (objectCollider != null) objectCollider.enabled = false;
            if (objectRenderer != null) objectRenderer.enabled = false;
        }
    }

    public void ConsumeWorldObject()
    {
        opened = true;

        // Solo desactiva colider y renderer si NO es la segunda pieza del reloj
        if (!isSecondClockPiece)
        {
            if (objectCollider != null) objectCollider.enabled = false;
            if (objectRenderer != null) objectRenderer.enabled = false;
        }
    }
}