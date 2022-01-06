using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Item
{
    [Header("Item Quantity")]
    public int maxItemAmount;
    public int currentItemAmount;

    [Header("Item Model")]
    public GameObject itemModel;

    [Header("Animations")]
    public string throwAnimation;

    [Header("Physics")]
    public float ForwardVelocity;
    public float UpwardVelocity;
    public float Mass;
    public bool isEffectedByGravity;

    public override string ColouredName => throw new System.NotImplementedException();

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

    public override string GetInfoDisplayText()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SuccessfullyConsumeItem(AnimatorManager animatorManager, RightHandHolderSlot rightHandHolderSlot, CameraManager cameraManager)
    {
        Debug.Log("you successfully throw item");
    }
}
