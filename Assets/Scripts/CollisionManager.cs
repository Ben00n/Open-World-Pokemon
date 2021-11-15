using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    Collider playerCollider;
    Animator animator;
    BattleHUD battleHUD;
    PokemonPartyManager pokemonPartyManager;
    BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        battleHUD = FindObjectOfType<BattleHUD>();
        animator = GetComponentInParent<Animator>();
        playerCollider = GetComponent<Collider>();
        playerCollider.isTrigger = true;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Pokemon")
        {
            PokemonStatsCalculator wildPokemonConfiguration = collision.GetComponent<PokemonStatsCalculator>();
            Animator pokemonAnimator = collision.GetComponentInChildren<Animator>();
            battleManager.wildPokemonStatsCalculator = collision.GetComponent<PokemonStatsCalculator>();
            battleManager.wildPokemonManager = collision.GetComponent<PokemonManager>();
            battleManager.wildPokemonAnimatorManager = collision.GetComponentInChildren<PokemonAnimatorManager>();

            if (wildPokemonConfiguration != null)
            {
                animator.SetBool("isInBattle", true);
                pokemonAnimator.SetBool("isInBattle", true);
                battleHUD.SetData(wildPokemonConfiguration,pokemonPartyManager.pokemons[0].transform.GetComponent<PokemonStatsCalculator>());
                pokemonPartyManager.pokemons[0].SetActive(true);
                pokemonPartyManager.pokemons[0].transform.localScale = new Vector3(1, 1, 1);
                pokemonPartyManager.pokemons[0].transform.position = transform.position;
                pokemonPartyManager.pokemons[0].transform.LookAt(Vector3.forward + pokemonPartyManager.pokemons[0].transform.position);


                transform.gameObject.SetActive(false);
            }
        }
    }
}
