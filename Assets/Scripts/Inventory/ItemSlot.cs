using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemSlot
{
    public InventoryItem item;
    public int quantity;

    public ItemSlot(InventoryItem item,int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
