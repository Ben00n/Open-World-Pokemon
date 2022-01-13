using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorSystem : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab = null;
    [SerializeField] private Transform buttonHolderTransform = null;

    [Header("Data Display")]
    [SerializeField] private TextMeshProUGUI itemNameText = null;
    [SerializeField] private Image itemIcon = null;
    [SerializeField] private TextMeshProUGUI itemDataText = null;

    private VendorData scenarioData = null;

    public void StartScenario(VendorData scenarioData)
    {
        ClearItemButtons();
        this.scenarioData = scenarioData;

        var items = scenarioData.SellingItemContainer.GetAllUniqueItems();

        for (int i = 0; i < items.Count; i++)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonHolderTransform);
            buttonInstance.GetComponent<VendorItemButton>().Initialise(this,items[i], scenarioData.SellingItemContainer.GetTotalQuantity(items[i]));
        }
        SetItem(scenarioData.SellingItemContainer.GetSlotByIndex(0).item);
    }

    public void SetItem(InventoryItem item)
    {
        itemNameText.text = item.Name;
        itemDataText.text = item.GetInfoDisplayText();
        itemIcon.sprite = item.Icon;
    }

    private void ClearItemButtons()
    {
        foreach (Transform child in buttonHolderTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
