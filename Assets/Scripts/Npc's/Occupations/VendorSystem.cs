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
    [SerializeField] private TextMeshProUGUI itemPrice = null;
    [SerializeField] private TextMeshProUGUI amountText = null;
    [SerializeField] private TextMeshProUGUI myMoneyAmount = null;

    protected int currentAmount = 1;
    private InventoryItem currentItem = null;

    private VendorData scenarioData = null;

    public void StartScenario(VendorData scenarioData)
    {
        ClearItemButtons();
        this.scenarioData = scenarioData;

        var items = scenarioData.SellingItemContainer.GetAllUniqueItems();

        for (int i = 0; i < items.Count; i++)
        {
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonHolderTransform);
            buttonInstance.GetComponent<VendorItemButton>().Initialise(this,items[i]);
        }
        SetItem(scenarioData.SellingItemContainer.GetSlotByIndex(0).item);
        SetMoneyAmount();
    }

    public void SetItem(InventoryItem item)
    {
        this.currentItem = item;
        currentAmount = 1;
        itemNameText.text = item.Name;
        itemDataText.text = item.GetInfoDisplayText();
        itemIcon.sprite = item.Icon;
        itemPrice.text = item.BuyPrice.ToString();
        amountText.text = currentAmount.ToString();
    }

    public void SetMoneyAmount()
    {
        myMoneyAmount.text = scenarioData.BuyingItemContainer.Money.ToString();
    }

    public void IncrementAmount()
    {
        if (currentAmount == 99)
            return;

        currentAmount++;
        amountText.text = currentAmount.ToString();
        itemPrice.text = (currentAmount * currentItem.BuyPrice).ToString();
    }

    public void DecrementAmount()
    {
        if (currentAmount == 1)
            return;

        currentAmount--;
        amountText.text = currentAmount.ToString();
        itemPrice.text = (currentAmount * currentItem.BuyPrice).ToString();
    }

    private void ClearItemButtons()
    {
        foreach (Transform child in buttonHolderTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ConfirmButton()
    {
        if (scenarioData.BuyingItemContainer.Money >= currentAmount * currentItem.BuyPrice)
        {
            scenarioData.BuyingItemContainer.Money -= currentAmount * currentItem.BuyPrice;

            scenarioData.BuyingItemContainer.AddItem(new ItemSlot(currentItem, currentAmount));
            SetMoneyAmount();
        }
        else
        {
            Debug.Log("You do not have enough money!");
        }    
    }
}
