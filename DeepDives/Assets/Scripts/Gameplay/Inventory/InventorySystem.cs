using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize = 99;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize = 99;
}

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class Inventory
{
    public List<InventorySlot> slots;
    public int maxSlots;

    public Inventory(int size)
    {
        maxSlots = size;
        slots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            slots.Add(null);
        }
    }

    public bool AddItem(ItemData item, int quantity)
    {
        // First, try to stack with existing items
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null && slots[i].item == item && slots[i].quantity < item.maxStackSize)
            {
                int spaceLeft = item.maxStackSize - slots[i].quantity;
                int toAdd = Mathf.Min(quantity, spaceLeft);
                slots[i].quantity += toAdd;
                quantity -= toAdd;
                if (quantity <= 0) return true;
            }
        }

        // Then, fill empty slots
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = new InventorySlot(item, quantity);
                return true;
            }
        }
        return false; // Inventory full
    }

    public bool RemoveItem(ItemData item, int quantity)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null && slots[i].item == item)
            {
                if (slots[i].quantity >= quantity)
                {
                    slots[i].quantity -= quantity;
                    if (slots[i].quantity <= 0)
                        slots[i] = null;
                    return true;
                }
            }
        }
        return false;
    }
}