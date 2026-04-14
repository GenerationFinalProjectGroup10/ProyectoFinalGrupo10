using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.OnItemListChanged += RefreshInventoryItems;
        RefreshInventoryItems(); // Pintar al inicio
    }

    private void RefreshInventoryItems()
    {
        // 🧠 1. Copiar y ordenar la lista correctamente
        List<Item> items = new List<Item>(inventory.GetItemList());
        items.Sort((a, b) => a.itemType.CompareTo(b.itemType));

        // 🧹 2. Limpiar slots anteriores
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        // 🎯 3. Crear slots nuevos
        int x = 0;
        int y = 0;
        float cellSize = 80f;

        foreach (Item item in items) // ✅ USAR la lista ordenada
        {
            RectTransform slotRect = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);

            // Posición en grilla
            slotRect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);

            // ⚠️ IMPORTANTE: nombre EXACTO del hijo
            Image image = slotRect.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}