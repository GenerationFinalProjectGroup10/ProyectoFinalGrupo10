using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory
{

    private List<Item> itemList;
    public event Action OnItemListChanged;

    public Inventory()
    {
        itemList = new List<Item>();

        AddItem(new Item { itemType = Item.ItemType.Key, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Coin, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Time, amount = 1 });
        Debug.Log(itemList.Count); 
    }

public void AddItem(Item item)
    {
        itemList.Add(item);
        OnItemListChanged?.Invoke();
    }

public List<Item> GetItemList()
    {
        return itemList;
    }

}