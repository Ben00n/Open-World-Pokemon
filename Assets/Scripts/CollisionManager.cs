using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    Collider playerTriggerCollider;
    Animator animator;
    BattleHUD battleHUD;
    PokemonPartyManager pokemonPartyManager;
    BattleManager battleManager;
    BattleDialogBox battleDialogBox;
    public Collider playerCollider;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        battleDialogBox = FindObjectOfType<BattleDialogBox>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        battleHUD = FindObjectOfType<BattleHUD>();
        animator = GetComponentInParent<Animator>();
        playerCollider = pokemonPartyManager.gameObject.GetComponent<Collider>();
        playerTriggerCollider = GetComponent<Collider>();
        playerTriggerCollider.isTrigger = true;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Pokemon")
        {
            battleManager.wildPokemonAnimator = collision.GetComponentInChildren<Animator>();
            battleManager.wildPokemonStatsCalculator = collision.GetComponent<PokemonStatsCalculator>();
            battleManager.wildPokemonManager = collision.GetComponent<PokemonManager>();
            battleManager.wildPokemonAnimatorManager = collision.GetComponentInChildren<PokemonAnimatorManager>();

            if (battleManager.wildPokemonStatsCalculator != null)
            {
                animator.SetBool("isInBattle", true);
                battleManager.wildPokemonAnimator.SetBool("isInBattle", true);
                battleHUD.SetData(battleManager.wildPokemonStatsCalculator,battleManager.playerPokemonStatsCalculator);


                PrepareMyPokemon();
                pokemonPartyManager.transform.position = battleManager.playerPokemonManager.transform.position + new Vector3(1, 0, 0); //set player position in battle
                playerCollider.enabled = false; // disable main collider for player

                battleDialogBox.SetDialog("Encountered a wild " + battleManager.wildPokemonStatsCalculator.pokemonBase.Name + "!");
                battleHUD.ActionSelector.SetActive(true);

                transform.gameObject.SetActive(false); // disable trigger collider as a gameobject

            }
        }
    }

    private void PrepareMyPokemon()
    {
        pokemonPartyManager.pokemons[0].SetActive(true);
        pokemonPartyManager.pokemons[0].transform.localScale = new Vector3(1, 1, 1);
        pokemonPartyManager.pokemons[0].transform.position = transform.position;
        pokemonPartyManager.pokemons[0].transform.LookAt(Vector3.forward + pokemonPartyManager.pokemons[0].transform.position);
    }
}
