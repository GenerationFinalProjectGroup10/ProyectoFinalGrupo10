using UnityEngine;

// ── Interfaz ─────────────────────────────────────────────────────────────────
// Todo ítem usable debe implementar esta interfaz.
// Así puedes crear llaves, pociones, cartas, palancas, etc.
public interface IUsable
{
    void Use(GameObject user);
}


// ── Llave ─────────────────────────────────────────────────────────────────────
// Crea un asset: clic derecho → Create → Inventory → Key Item
[CreateAssetMenu(fileName = "NewKey", menuName = "Inventory/Key Item")]
public class KeyItem : ItemData, IUsable
{
    [Header("ID de la puerta que abre")]
    public string targetDoorID; // Debe coincidir con el DoorController

    public void Use(GameObject user)
    {
        // Busca la puerta por su ID en la escena
        DoorController[] doors = Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None);

        foreach (var door in doors)
        {
            if (door.doorID == targetDoorID)
            {
                door.TryOpen(this);
                return;
            }
        }

        Debug.Log($"No hay puerta con ID '{targetDoorID}' en la escena.");
    }
}