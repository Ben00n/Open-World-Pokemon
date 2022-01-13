using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorItemButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText = null;
    [SerializeField] private Image itemIconImage = null;

    private VendorSystem vendorSystem = null;
    private InventoryItem item = null;

    public void Initialise(VendorSystem vendorSystem, InventoryItem item, int quantity)
    {
        this.vendorSystem = vendorSystem;
        this.item = item;
        itemIconImage.sprite = item.Icon;
        itemNameText.text = $"{item.Name}";
    }

    public void SelectItem()
    {
        vendorSystem.SetItem(item);
    }
}
