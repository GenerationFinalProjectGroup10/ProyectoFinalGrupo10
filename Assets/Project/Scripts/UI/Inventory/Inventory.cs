using System;
using System.Collections.Generic;


public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public event Action OnInventoryChanged;


    public void AddItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;


        InventoryItem existing = items.Find(i => i.item == item);


        if (existing != null)
            existing.amount += amount;
        else
            items.Add(new InventoryItem { item = item, amount = amount });


        OnInventoryChanged?.Invoke();
    }


    public bool HasItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return false;


        InventoryItem existing = items.Find(i => i.item == item);
        return existing != null && existing.amount >= amount;
    }


    public bool RemoveItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return false;


        InventoryItem existing = items.Find(i => i.item == item);


        if (existing != null && existing.amount >= amount)
        {
            existing.amount -= amount;


            if (existing.amount <= 0)
                items.Remove(existing);


            OnInventoryChanged?.Invoke();
            return true;
        }


        return false;
    }
}