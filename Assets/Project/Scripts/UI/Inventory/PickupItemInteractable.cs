using UnityEngine;

public class PickupItemInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int amount = 1;
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField] private GameObject pickupVfxPrefab;
    [SerializeField] private float vfxLifetime = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float pickupVolume = 1f;

    public string GetInteractMessage()
    {
        return "Presiona E para recoger";
    }

    public void Interact(PlayerController player)
    {
        if (item == null) return;
        if (InventoryManager.Instance == null) return;

        InventoryManager.Instance.AddItem(item, amount);

        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);

        if (pickupVfxPrefab != null)
        {
            GameObject fx = Instantiate(pickupVfxPrefab, transform.position, Quaternion.identity);
            Destroy(fx, vfxLifetime);
        }

        Destroy(gameObject);
    }
}