using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/PokeBalls")]
public class PokeBall : ConsumableItem
{
    [Header("Pokeball Type")]
    public bool normalPokeBall;

    public float catchRate;

    [Header("PokeBall Physics")]
    public float pokeBallForwardVelocity;
    public float pokeBallUpwardVelocity;
    public float pokeBallMass;
    public bool isEffectedByGravity;
    Rigidbody rigidBody;

    public override void AttemptToConsumeItem(AnimatorManager AnimatorManager,RightHandHolderSlot rightHandHolderSlot, CameraManager cameraManager)
    {
        base.AttemptToConsumeItem(AnimatorManager,rightHandHolderSlot,cameraManager);


        //GameObject pokeBall = Instantiate(itemModel, rightHandHolderSlot.transform);
        //pokeBall.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

    }

    public override void SuccessfullyConsumeItem(AnimatorManager animatorManager, RightHandHolderSlot rightHandHolderSlot, CameraManager cameraManager)
    {
        base.SuccessfullyConsumeItem(animatorManager, rightHandHolderSlot,cameraManager);
        GameObject pokeBall = Instantiate(itemModel, rightHandHolderSlot.transform.position,cameraManager.cameraPivot.rotation);
        rigidBody = pokeBall.GetComponent<Rigidbody>();

        rigidBody.AddForce(pokeBall.transform.forward * pokeBallForwardVelocity);
        rigidBody.AddForce(pokeBall.transform.up * pokeBallUpwardVelocity);
        rigidBody.useGravity = isEffectedByGravity;
        rigidBody.mass = pokeBallMass;
        pokeBall.transform.parent = null;
    }
}
