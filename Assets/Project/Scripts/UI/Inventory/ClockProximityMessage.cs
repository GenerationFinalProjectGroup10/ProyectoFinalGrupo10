using UnityEngine;

public class ClockProximityMessage : MonoBehaviour
{
    [SerializeField] private ItemSO requiredClockPart;
    [SerializeField] private string message = "Junta las cuatro partes del portaretrato para obtener la manecilla del reloj";

    private bool playerInside;
    private bool messageShown;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        playerInside = true;
        UpdateMessage();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        UpdateMessage();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        playerInside = false;
        messageShown = false;
        UI_Message.Instance?.HideInteraction();
    }

    private void UpdateMessage()
    {
        if (!playerInside) return;
        if (InventoryManager.Instance == null || InventoryManager.Instance.inventory == null) return;

        bool hasManecilla = requiredClockPart != null && InventoryManager.Instance.inventory.HasItem(requiredClockPart, 1);
        if (hasManecilla)
        {
            messageShown = false;
            return;
        }

        if (!messageShown)
        {
            UI_Message.Instance?.ShowInteraction(message);
            messageShown = true;
        }
    }
}