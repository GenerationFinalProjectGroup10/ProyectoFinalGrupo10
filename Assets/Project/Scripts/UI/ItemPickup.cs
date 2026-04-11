using UnityEngine;

// Pon este script en cada objeto 3D que el player pueda recoger.
// El GameObject necesita un Collider con "Is Trigger" activado.
public class ItemPickup : MonoBehaviour
{
    [Header("Ítem que representa este objeto")]
    public ItemData itemData;

    [Header("Efectos opcionales")]
    public GameObject pickupEffectPrefab; // Partículas al recoger (opcional)
    public AudioClip pickupSound;

    private bool pickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        // Solo reacciona al player (asegúrate de que tu player tenga el tag "Player")
        if (pickedUp || !other.CompareTag("Player")) return;

        bool added = InventoryManager.Instance.AddItem(itemData);

        if (added)
        {
            pickedUp = true;

            // Feedback visual/sonoro
            if (pickupEffectPrefab)
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Destruir el objeto del mundo
            Destroy(gameObject);
        }
    }

    // Muestra el nombre en el Editor para saber qué es el objeto
    void OnDrawGizmosSelected()
    {
        if (itemData != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, itemData.itemName);
        }
    }
}