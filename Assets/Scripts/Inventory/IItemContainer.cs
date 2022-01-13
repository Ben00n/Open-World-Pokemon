using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemContainer
{
    ItemSlot GetSlotByIndex(int index);
    ItemSlot AddItem(ItemSlot itemSlot);
    List<InventoryItem> GetAllUniqueItems();
    void RemoveItem(ItemSlot itemSlot);
    void RemoveAt(int slotIndex);
    void Swap(int indexOne, int indexTwo);
    bool HasItem(InventoryItem item);
    int GetTotalQuantity(InventoryItem item);
}
