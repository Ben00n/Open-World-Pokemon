using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Throwables")]
public class Throwable : ConsumableItem
{
    [Header("Item Model")]
    public GameObject itemModel;

    [Header("Animations")]
    public string consumeAnimation;

    [Header("Physics")]
    public float ForwardVelocity;
    public float UpwardVelocity;
    public float Mass;
    public bool isEffectedByGravity;

    Rigidbody rigidBody;

    public void AttemptToConsumeItem(AnimatorManager animatorManager,RightHandHolderSlot rightHandHolderSlot,CameraManager cameraManager)
    {
        animatorManager.PlayTargetAnimation(consumeAnimation, true);
    }

    public void SuccessfullyConsumeItem(AnimatorManager animatorManager, RightHandHolderSlot rightHandHolderSlot, CameraManager cameraManager)
    {
        GameObject pokeBall = Instantiate(itemModel, rightHandHolderSlot.transform.position, cameraManager.cameraPivot.rotation);
        rigidBody = pokeBall.GetComponent<Rigidbody>();

        rigidBody.AddForce(pokeBall.transform.forward * ForwardVelocity);
        rigidBody.AddForce(pokeBall.transform.up * UpwardVelocity);
        rigidBody.useGravity = isEffectedByGravity;
        rigidBody.mass = Mass;
        pokeBall.transform.parent = null;
    }
}
