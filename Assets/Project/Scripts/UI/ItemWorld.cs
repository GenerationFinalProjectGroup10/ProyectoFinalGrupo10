using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        if (ItemAssets.Instance.pfItemWorld == null)
        {
            Debug.LogError("pfItemWorld not assigned in ItemAssets!");
            return null;
        }

        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;

    // 🔥 Bandera para evitar recoger el item dos veces
    private bool isPicked = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
    }

    // Devuelve el item real (sin duplicar cantidad)
    public Item GetItem()
    {
        // Si ya lo recogieron, no volver a entregarlo
        if (isPicked) return null;

        isPicked = true; // 🔥 Marca como recogido

        // Devuelve una copia del item
        return new Item
        {
            itemType = item.itemType,
            amount = item.amount
        };
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}