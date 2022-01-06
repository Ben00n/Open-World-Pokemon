using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public RightHandHolderSlot rightHandHolderSlot;
    PlayerControls playerControls;
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;
    CameraManager cameraManager;
    UIManager uiManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public bool lockOnInput;
    public bool lockOnFlag;

    public bool isLooking;

    public bool R_InputFlag;
    public bool R_Input;

    public bool I_Input;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool shift_Input;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraLooking.performed += i => isLooking = true;
            playerControls.PlayerMovement.CameraLooking.canceled += i => isLooking = false;
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.ThrowPokeBall.performed += i => R_Input = true;
            playerControls.PlayerActions.OpenInventory.performed += i => I_Input = true;

            playerControls.PlayerActions.Shift.performed += i => shift_Input = true;
            playerControls.PlayerActions.Shift.canceled += i => shift_Input = false;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleLockOnInput();
        HandlePokeBallInput();
        HandleInventoryInput();
        //handle jump etc
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        if (playerManager.isInBattle)
            return;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (shift_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleLockOnInput()
    {
        if (lockOnInput && lockOnFlag == false && !playerManager.isInBattle)
        {

            lockOnInput = false;
            cameraManager.HandleLockOn();
            if (cameraManager.nearestLockOnTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraManager.ClearLockOnTargets();
        }
    }

    private void HandlePokeBallInput()
    {
        if(lockOnFlag && R_Input && !R_InputFlag)
        {
            R_InputFlag = true;
            R_Input = false;
            playerManager.currentThrowable.AttemptToConsumeItem(animatorManager, rightHandHolderSlot,cameraManager);
            R_InputFlag = false;
        }
    }

    private void HandleInventoryInput()
    {
        if (I_Input)
        {
            uiManager.HandleInventory();
        }
    }

    private void SuccessfullyThrow() // called in throw animation as an event
    {
        playerManager.currentThrowable.SuccessfullyConsumeItem(animatorManager, rightHandHolderSlot, cameraManager);
    }
}
