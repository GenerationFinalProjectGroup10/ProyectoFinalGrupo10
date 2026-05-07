using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTempTest : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private ItemSO requiredKey;
    [SerializeField] private ItemSO requiredFrame;
    [SerializeField] private ItemSO requiredBear;
    
    [Header("Settings")]
    [SerializeField] private string nextSceneName = "SampleScene";
    
    private Collider doorCollider;
    private bool opened;

    private void Awake() => doorCollider = GetComponent<Collider>();

    private string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public string GetInteractMessage()
    {
        if (opened) return "";
        var inv = InventoryManager.Instance.inventory;
        string scene = GetCurrentSceneName();

        bool hasKey = inv.HasItem(requiredKey, 1);
        bool hasFrame = inv.HasItem(requiredFrame, 1);
        bool hasBear = inv.HasItem(requiredBear, 1);

        if (nextSceneName == "Mundo1" && hasFrame && hasBear)
        {
            return "Presiona E";
        }
        
        if (scene == "Sotano 1")
        {
            return (hasBear && hasFrame) ? "Presiona E" : "Necesitas el Oso y el ojo";
        }
        else if (scene == "Mundo3 1")
        {
            return (hasFrame && hasKey) ? "Presiona E para abrir" : "Necesitas el ojo y la Llave";
        }
        
        return hasKey ? "Presiona E para abrir" : "Necesitas la Llave";
    }

    public void Interact(PlayerController player)
    {
        if (opened || InventoryManager.Instance == null) return;

        var inv = InventoryManager.Instance.inventory;
        string scene = GetCurrentSceneName();
        
        bool hasKey = inv.HasItem(requiredKey, 1);
        bool hasFrame = inv.HasItem(requiredFrame, 1);
        bool hasBear = inv.HasItem(requiredBear, 1);

        // Lógica de Juego Terminado
        if (nextSceneName == "Mundo1" && hasFrame && hasBear)
        {
            OpenDoor("¡Juego terminado!");
            return;
        }

        if (scene == "Sotano 1")
        {
            if (hasBear && hasFrame)
            {
                inv.RemoveItem(requiredBear, 1);
                inv.RemoveItem(requiredFrame, 1);
                OpenDoor("¡Puerta abierta!");
            }
            else UI_Message.Instance?.ShowTemporary("Necesitas el oso completo para abrir.", 2f);
        }
        else if (scene == "Mundo3 1")
        {
            if (hasFrame && hasKey)
            {
                inv.RemoveItem(requiredKey, 1);
                OpenDoor("¡Puerta abierta!");
            }
            else UI_Message.Instance?.ShowTemporary("Necesitas el ojo y la Llave para abrir.", 2f);
        }
        else 
        {
            if (hasKey)
            {
                inv.RemoveItem(requiredKey, 1);
                OpenDoor("¡Puerta abierta!");
            }
            else UI_Message.Instance?.ShowTemporary("Necesitas la Llave para abrir.", 2f);
        }
    }

    // El mensaje es opcional y dinámico
    private void OpenDoor(string message = "¡Puerta abierta!")
    {
        opened = true;
        UI_Message.Instance?.ShowTemporary(message, 2.5f);
        if (doorCollider != null) doorCollider.enabled = false;
        Invoke(nameof(ChangeScene), 2.5f);
    }

    private void ChangeScene() => SceneManager.LoadScene(nextSceneName);
}