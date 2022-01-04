using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : Item
{
    [Header("Item Quantity")]
    public int maxItemAmount;
    public int currentItemAmount;

    [Header("Item Model")]
    public GameObject itemModel;

    [Header("Animations")]
    public string consumeAnimation;


    public virtual void AttemptToConsumeItem(AnimatorManager animatorManager,RightHandHolderSlot rightHandHolderSlot,CameraManager cameraManager)
    {
        if (currentItemAmount > 0)
        {
            animatorManager.PlayTargetAnimation("Throw", true);
        }
        else
        {
            Debug.Log("failed no quantity");
        }
    }

    public virtual void SuccessfullyConsumeItem(AnimatorManager animatorManager, RightHandHolderSlot rightHandHolderSlot, CameraManager cameraManager)
    {
        Debug.Log("you successfully consumed item");
    }
}
