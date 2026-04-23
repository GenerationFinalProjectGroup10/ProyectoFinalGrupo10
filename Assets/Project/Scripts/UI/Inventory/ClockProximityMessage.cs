using UnityEngine;


public class ClockProximityMessage : MonoBehaviour
{
    [SerializeField] private ItemSO requiredClockPart;
    [SerializeField] private string message = "Debes encontrar la manecilla";


    private bool playerInside;


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
        UI_Message.Instance?.HideInteraction();
    }


    private void UpdateMessage()
    {
        if (!playerInside) return;
        if (InventoryManager.Instance == null) return;
        if (InventoryManager.Instance.inventory == null) return;


        bool hasManecilla = requiredClockPart != null &&
                  InventoryManager.Instance.inventory.HasItem(requiredClockPart, 1);


        if (!hasManecilla)
        {
            UI_Message.Instance?.ShowInteraction(message);
        }
    }
}