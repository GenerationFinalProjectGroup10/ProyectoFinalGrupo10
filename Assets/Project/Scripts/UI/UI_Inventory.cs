using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");

        itemSlotTemplate.gameObject.SetActive(false);

        Debug.Log("Template encontrado: " + itemSlotTemplate.name);
    }

    private void Start()
    {
        // Asegurar que el template nunca aparezca cuando inicie la escena
        itemSlotTemplate.gameObject.SetActive(false);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        // Suscripción correcta
        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems(); // Pintar al inicio
    }

    // Evento compatible con EventHandler
    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        // Limpia slots anteriores
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        if (inventory == null) return;

        // Copiar y ordenar la lista
        List<Item> items = new List<Item>(inventory.GetItemList());
        items.Sort((a, b) => a.itemType.CompareTo(b.itemType));

        int x = 0;
        int y = 0;
        float cellSize = 80f;

        foreach (Item item in items)
        {
            RectTransform slotRect = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            slotRect.gameObject.SetActive(true);

            // Posición de grilla
            slotRect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);

            // Asignar sprite del item
            Image image = slotRect.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            // Muestra cantidad de items
            TextMeshProUGUI uiText = slotRect.Find("AmountText").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
            {
                uiText.SetText(item.amount.ToString());
            }
            else
            {
                uiText.SetText("");
            }

            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}