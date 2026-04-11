using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Requiere: Canvas con un panel "inventoryPanel" y un prefab de slot
// El slot prefab necesita: Image (ícono) + Button + TextMeshPro (nombre)
public class InventoryUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject inventoryPanel;
    public Transform slotsContainer;    // El grid/layout donde van los slots
    public GameObject slotPrefab;       // Prefab de cada casilla

    private bool isOpen = false;
    private List<GameObject> slotInstances = new List<GameObject>();

    void Start()
    {
        inventoryPanel.SetActive(false);

        // Suscribirse a cambios del inventario
        InventoryManager.Instance.OnInventoryChanged.AddListener(RefreshUI);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
            // Opcional: pausar el juego o liberar el cursor
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    void RefreshUI()
    {
        // Limpiar slots anteriores
        foreach (var slot in slotInstances)
            Destroy(slot);
        slotInstances.Clear();

        // Crear un slot por cada ítem
        List<ItemData> items = InventoryManager.Instance.GetAllItems();
        foreach (ItemData item in items)
        {
            GameObject slot = Instantiate(slotPrefab, slotsContainer);
            slotInstances.Add(slot);

            // Asignar ícono
            Image icon = slot.GetComponentInChildren<Image>();
            if (icon && item.icon) icon.sprite = item.icon;

            // Asignar nombre
            TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = item.itemName;

            // Al hacer clic → usar el ítem
            Button btn = slot.GetComponent<Button>();
            if (btn)
            {
                ItemData captured = item; // Captura para el lambda
                btn.onClick.AddListener(() => UseItemFromUI(captured));
            }
        }
    }

    void UseItemFromUI(ItemData item)
    {
        GameObject player = GameObject.FindWithTag("Player");
        InventoryManager.Instance.UseItem(item, player);
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged.RemoveListener(RefreshUI);
    }
}