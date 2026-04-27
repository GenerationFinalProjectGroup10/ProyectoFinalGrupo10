using UnityEngine;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");

        if (itemSlotContainer == null)
        {
            Debug.LogError("UI_Inventory: No se encontró 'ItemSlotContainer'");
            return;
        }

        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");

        if (itemSlotTemplate == null)
        {
            Debug.LogError("UI_Inventory: No se encontró 'ItemSlotTemplate'");
            return;
        }

        itemSlotTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (InventoryManager.Instance != null)
            SetInventory(InventoryManager.Instance.inventory);
        else
            Debug.LogError("UI_Inventory: No existe InventoryManager.Instance");
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshUI;
    }

    public void SetInventory(Inventory inventory)
    {
        if (this.inventory != null)
            this.inventory.OnInventoryChanged -= RefreshUI;

        this.inventory = inventory;

        if (this.inventory != null)
        {
            this.inventory.OnInventoryChanged += RefreshUI;
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (inventory == null || itemSlotContainer == null || itemSlotTemplate == null) return;

        //log en consola al abrir la puerta
        Debug.Log("UI_Inventory: Refrescando UI. Items actuales: " + inventory.items.Count);

        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invItem in inventory.items)
        {
            Transform slot = Instantiate(itemSlotTemplate, itemSlotContainer);
            slot.gameObject.SetActive(true);

            // Verifica que los componentes existan antes de acceder
            var icon = slot.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
            var text = slot.Find("AmountText")?.GetComponent<TextMeshProUGUI>();

            if (icon != null) icon.sprite = invItem.item.icon;
            if (text != null) text.text = invItem.amount.ToString();
        }
    }
}