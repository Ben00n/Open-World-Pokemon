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
    public bool isFainted;

    private float targetScale = 0.01f;
    private float shrinkSpeed = 3f;
    public bool shrinking = false;

    private void Awake()
    {
        pokemonLocomotion = GetComponent<PokemonLocomotion>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        isInBattle = animator.GetBool("isInBattle");
        isAttacking = animator.GetBool("isAttacking");
        isFainted = animator.GetBool("isFainted");

        if (shrinking) // called in pokeballcollider
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * shrinkSpeed);
        }

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
