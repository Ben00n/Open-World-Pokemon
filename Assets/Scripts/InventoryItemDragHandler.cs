using UnityEngine.EventSystems;

public class InventoryItemDragHandler : ItemDragHandler
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData);

            if(eventData.hovered.Count == 0)
            {
                //destory item or drop item
            }
        }
    }
}
