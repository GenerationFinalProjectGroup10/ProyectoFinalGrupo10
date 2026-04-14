using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged;

    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        Debug.Log(itemList.Count);
    }

    public void AddItem(Item item)
    {
        if (item.IsStackable())
        {
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += 1; // SIEMPRE suma 1
                    OnItemListChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // Nuevo item siempre inicia con 1
        itemList.Add(new Item
        {
            itemType = item.itemType,
            amount = 1
        });

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    // ⭐⭐ ESTE ERA EL MÉTODO QUE TE FALTABA ⭐⭐
    public List<Item> GetItemList()
    {
        return itemList;
    }
}