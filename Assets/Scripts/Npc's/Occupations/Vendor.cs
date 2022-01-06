using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : MonoBehaviour, IOccupation
{
    public string Name => "Let's Trade";
    public string Data
    {
        get
        {
            string itemNames = "";
            List<Item> items = itemContainer.GetAllItems();
            for (int i = 0; i < items.Count; i++)
            {
                itemNames += $"{items[i].name}, ";
            }

            return itemNames;
        }
    }

    private IItemContainer itemContainer = null;

    private void Start() => itemContainer = GetComponent<IItemContainer>();
}
