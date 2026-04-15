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

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType && inventoryItem.amount > 0)
                {
                    inventoryItem.amount -= 1;
                    if (inventoryItem.amount <= 0)
                    {
                        itemList.Remove(inventoryItem);
                    }
                    OnItemListChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        else
        {
            // For non-stackable, remove the first occurrence
            Item toRemove = itemList.Find(i => i.itemType == item.itemType);
            if (toRemove != null)
            {
                itemList.Remove(toRemove);
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public List<Item> GetItemList()
    {
        return new List<Item>(itemList);
    }
}