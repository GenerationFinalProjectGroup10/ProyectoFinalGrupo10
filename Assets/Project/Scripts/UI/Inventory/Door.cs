using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO requiredKey;
    [SerializeField] private string nextSceneName = "SampleScene";
    private Collider doorCollider;
    private bool opened;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }

    // ✅ MÉTODOS COMPLETOS de IInteractable
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

    // ✅ MÉTODO FALTANTE - Agregar este
    public bool CanInteract()
    {
        return !opened && InventoryManager.Instance != null && requiredKey != null;
    }

    private void OpenDoor()
    {
        opened = true;
        UI_Message.Instance?.ShowTemporary("¡Puerta abierta!", 2.5f);

        if (doorCollider != null) doorCollider.enabled = false;
        Invoke("ChangeScene", 2.5f);
    }

    private void ChangeScene()
    {
        Debug.Log($"Cargando escena: '{nextSceneName}'");
        SceneManager.LoadScene(nextSceneName);
    }
}