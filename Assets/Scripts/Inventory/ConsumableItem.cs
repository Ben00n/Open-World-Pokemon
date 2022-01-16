using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item",menuName = "Consumable Item")]
public class ConsumableItem : InventoryItem, IUseable
{
    [SerializeField] private UseableEvent onUseablePressed = null;

    [Header("Consumable Data")]
    [SerializeField] private string useText = "Does something?";

    public override string GetInfoDisplayText()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Rarity.Name).AppendLine();
        builder.Append("<color=green>Use: ").Append(useText).Append("</color>").AppendLine();
        builder.Append("Max Stack: ").Append(MaxStack).AppendLine();

        return builder.ToString();
    }
    public void Use()
    {
        onUseablePressed.Raise(this);
        Debug.Log("test");
    }
}
