using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonAnimatorManager : MonoBehaviour
{
    PokemonLocomotion pokemonLocomotion;
    public Animator animator;

    private void Awake()
    {
        pokemonLocomotion = GetComponentInParent<PokemonLocomotion>();
        animator = GetComponent<Animator>();
    }
    public void PlayTargetAnimation(string targetAnim)
    {
        //animator.applyRootMotion = isInteracting;
        //animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        pokemonLocomotion.pokemonRigidBody.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        pokemonLocomotion.pokemonRigidBody.velocity = velocity;
    }
}
