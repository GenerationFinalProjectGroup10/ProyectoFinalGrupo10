using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private ItemSO requiredKey;
    [SerializeField] private ItemSO requiredFrame;

    [Header("Settings")]
    [SerializeField] private string nextSceneName = "SampleScene";

    private Collider doorCollider;
    private bool opened;

    private void Awake() => doorCollider = GetComponent<Collider>();

    private bool IsSotanoScene()
    {
        return SceneManager.GetActiveScene().name == "Sotano 1";
    }

    public string GetInteractMessage()
    {
        if (opened) return "";
        var inv = InventoryManager.Instance.inventory;
        bool hasKey = inv.HasItem(requiredKey, 1);

        if (IsSotanoScene())
        {
            bool hasFrame = inv.HasItem(requiredFrame, 1);
            return (hasKey && hasFrame) ? "Presiona E para abrir" : "Necesitas el Frame y la Llave";
        }
        return hasKey ? "Presiona E para abrir" : "Necesitas la Llave";
    }

    public void Interact(PlayerController player)
    {
        if (opened || InventoryManager.Instance == null) return;

        var inv = InventoryManager.Instance.inventory;
        bool hasKey = inv.HasItem(requiredKey, 1);
        bool hasFrame = inv.HasItem(requiredFrame, 1);

        if (IsSotanoScene())
        {
            if (hasKey && hasFrame)
            {
                // DEPURACIÓN: Vamos a ver si realmente entra aquí y qué intenta borrar
                Debug.Log($"Intentando eliminar: Key={requiredKey.name}, Frame={requiredFrame.name}");

                bool keyRemoved = inv.RemoveItem(requiredKey, 1);
                bool frameRemoved = inv.RemoveItem(requiredFrame, 1);

                Debug.Log($"Resultado: ¿Llave borrada? {keyRemoved} | ¿Frame borrado? {frameRemoved}");

                OpenDoor();
            }
            else
            {
                UI_Message.Instance?.ShowTemporary("Necesitas el Frame y la Llave para abrir.", 2f);
            }
        }
        else // Lógica para otras escenas
        {
            if (hasKey)
            {
                inv.RemoveItem(requiredKey, 1);
                OpenDoor();
            }
            else
            {
                UI_Message.Instance?.ShowTemporary("Necesitas la Llave para abrir.", 2f);
            }
        }
    }

    private void OpenDoor()
    {
        opened = true;
        UI_Message.Instance?.ShowTemporary("¡Puerta abierta!", 2.5f);
        if (doorCollider != null) doorCollider.enabled = false;
        Invoke(nameof(ChangeScene), 2.5f);
    }

    private void ChangeScene() => SceneManager.LoadScene(nextSceneName);
}