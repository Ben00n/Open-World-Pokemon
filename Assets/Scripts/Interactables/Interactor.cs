using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    InputManager inputManager;
    private IInteractable currentInteractable = null;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        if (currentInteractable == null) { return; }

        if(inputManager.F_Input)
        {
            currentInteractable.Interact(transform.root.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();

        if (interactable == null) { return; }

        currentInteractable = interactable;
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();

        if (interactable == null) { return; }

        if (interactable != currentInteractable) { return; }

        currentInteractable = null;
    }
}
