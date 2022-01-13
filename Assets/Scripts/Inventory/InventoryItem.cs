using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : Item
{
    [Header("Item Data")]
    [SerializeField] private Rarity rarity = null;
    [SerializeField] [Min(0)] private int buyPrice = 1;
    [SerializeField] [Min(1)] private int maxStack = 1;

    public override string ColouredName
    {
        get
        {
            string hexColour = ColorUtility.ToHtmlStringRGB(rarity.TextColour);

            return $"<color=#{hexColour}>{Name}</color>";
        }
    }
    public int BuyPrice => buyPrice;
    public int MaxStack => maxStack;
    public Rarity Rarity => rarity;

}
