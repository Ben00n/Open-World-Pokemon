using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("CurrentPokeBall")]
    public Throwable currentThrowable;

    public Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    [Header("Flags")]
    public bool isInBattle;

    private void Awake()
    {
        ConditionsDB.Init();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        isInBattle = animator.GetBool("isInBattle");
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        inputManager.R_Input = false;
        inputManager.I_Input = false;
        inputManager.F_Input = false;

        cameraManager.HandleAllCameraMovement();
    }
}
