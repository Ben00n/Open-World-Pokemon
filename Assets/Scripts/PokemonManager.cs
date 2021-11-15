using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : MonoBehaviour
{
    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    PokemonLocomotion pokemonLocomotion;
    Animator animator;
    public bool isInBattle;
    public bool isAttacking;

    private void Awake()
    {
        pokemonLocomotion = GetComponent<PokemonLocomotion>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        isInBattle = animator.GetBool("isInBattle");
        isAttacking = animator.GetBool("isAttacking");
    }

    private void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void LateUpdate()
    {
        
    }

    private void HandleCurrentAction()
    {
        pokemonLocomotion.HandleRandomMovement();
    }
}
