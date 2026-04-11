using System.Collections;
using UnityEngine;

// Pon este script en el GameObject de la puerta.
// Necesita un Collider Trigger para detectar al player.
public class DoorController : MonoBehaviour
{
    [Header("Identificación")]
    public string doorID = "door_01"; // Debe coincidir con KeyItem.targetDoorID

    [Header("Animación de apertura")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public AudioClip openSound;

    private bool isOpen = false;
    private bool playerNearby = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);
    }

    void Update()
    {
        // El player presiona E cerca de la puerta
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            TryOpenWithInventory();
    }

    // ── Llamado desde KeyItem.Use() ───────────────────────────────
    public void TryOpen(KeyItem key)
    {
        if (isOpen) return;

        if (key.targetDoorID == doorID)
        {
            Debug.Log($"Puerta '{doorID}' abierta con '{key.itemName}'.");
            Open();
        }
        else
        {
            Debug.Log("Esta llave no abre esta puerta.");
        }
    }

    // ── Abre la puerta con lo que haya en el inventario ──────────
    void TryOpenWithInventory()
    {
        if (isOpen) return;

        var inv = InventoryManager.Instance;
        if (inv.HasItem(doorID)) // Convenio: itemID de la llave = doorID
        {
            ItemData key = inv.GetItem(doorID);
            inv.UseItem(key, null);
        }
        else
        {
            Debug.Log("Necesitas la llave correcta.");
            // Aquí puedes mostrar un mensaje en UI
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        if (openSound) AudioSource.PlayClipAtPoint(openSound, transform.position);
        StartCoroutine(AnimateOpen());
    }

    IEnumerator AnimateOpen()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(closedRotation, openRotation, t);
            yield return null;
        }
        transform.rotation = openRotation;
    }

    // ── Detectar proximidad del player ───────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log($"[{doorID}] Presiona E para intentar abrir.");
            // Aquí puedes activar un prompt en UI: "Presiona E"
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}