using DapperDino.DapperTools.ScriptableEvents.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private VoidEvent onInventoryItemsUpdated = null;
    [SerializeField] private ItemSlot testItemSlot = new ItemSlot();

    public ItemContainer ItemContainer { get; } = new ItemContainer(20);

    public void OnEnable() => ItemContainer.OnItemsUpdated += onInventoryItemsUpdated.Raise;

    private void OnDisable() => ItemContainer.OnItemsUpdated -= onInventoryItemsUpdated.Raise;

    [ContextMenu("test")]
    public void TestAdd()
    {
        ItemContainer.AddItem(testItemSlot);
    }
}
