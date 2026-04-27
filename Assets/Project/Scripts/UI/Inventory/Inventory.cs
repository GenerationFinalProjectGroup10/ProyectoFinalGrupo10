using System;
using System.Collections.Generic;

public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public event Action OnInventoryChanged;

    public void AddItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;

        // Busca por nombre de item para evitar problemas con referencias
        InventoryItem existing = items.Find(i => i.item.name == item.name);

        if (existing != null)
            existing.amount += amount;
        else
            items.Add(new InventoryItem { item = item, amount = amount });

        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return false;

        // Buscamos por nombre
        InventoryItem existing = items.Find(i => i.item.name == item.name);
        return existing != null && existing.amount >= amount;
    }

    public bool RemoveItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return false;

        // Buscamos por nombre para asegurar la eliminación correcta
        InventoryItem existing = items.Find(i => i.item.name == item.name);

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