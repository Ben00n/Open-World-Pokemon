﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleManager : MonoBehaviour
{
    CollisionManager collisionManager;
    PlayerManager playerManager;
    BattleDialogBox battleDialogBox;
    BattleHUD battleHUD;
    PokemonPartyManager pokemonPartyManager;
    [SerializeField] PartyScreen partyScreen;

    [Header("Player Pokemon Components")]
    public PokemonStatsCalculator playerPokemonStatsCalculator;
    public PokemonAnimatorManager playerPokemonAnimatorManager;
    public PokemonManager playerPokemonManager;
    public Animator playerPokemonAnimator;

    [Header("Wild Pokemon Components")]
    public PokemonStatsCalculator wildPokemonStatsCalculator;
    public PokemonAnimatorManager wildPokemonAnimatorManager;
    public PokemonManager wildPokemonManager;
    public Animator wildPokemonAnimator;

    private void Awake()
    {
        collisionManager = FindObjectOfType<CollisionManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        battleDialogBox = FindObjectOfType<BattleDialogBox>();
        battleHUD = FindObjectOfType<BattleHUD>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
    }

    public void PerformMove1()
    {
        StartCoroutine(PerformPlayerMove1());
    }

    IEnumerator PerformPlayerMove1()
    {
        var move = playerPokemonStatsCalculator.Moves[0];
        if (move.Base.isPhysicalAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.Base.isSpecialAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }
        yield return RunMove(playerPokemonStatsCalculator, wildPokemonStatsCalculator, move);
    }

    public void PerformMove2()
    {
        //StartCoroutine(PerformPlayerMove2());
    }

    public void PerformMove3()
    {
        //StartCoroutine(PerformPlayerMove3());
    }

    public void PerformMove4()
    {
        //StartCoroutine(PerformPlayerMove4());
    }

    IEnumerator EnemyMove()
    {
        var move = wildPokemonStatsCalculator.GetRandomMove();
        move.PP--;
        if (move.Base.isSpecialAttack)
        {
            wildPokemonManager.isAttacking = true;
            wildPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }
        if (move.Base.isPhysicalAttack)
        {
            wildPokemonManager.isAttacking = true;
            wildPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }
        yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} used {move.Base.Name}");

        var damageDetails = playerPokemonStatsCalculator.TakeDamage(move.Base, wildPokemonStatsCalculator);
        yield return battleHUD.UpdatePokemonHP(playerPokemonStatsCalculator,battleHUD.playerPokemonHPBar);
        yield return ShowDamageDetails(damageDetails);


        if (damageDetails.Fainted)
        {
            yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} Fainted");
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
            var nextPokemon = pokemonPartyManager.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                nextPokemon.transform.position = playerPokemonStatsCalculator.transform.position;

                playerPokemonAnimatorManager = nextPokemon.GetComponentInChildren<PokemonAnimatorManager>();
                playerPokemonManager = nextPokemon.GetComponent<PokemonManager>();
                playerPokemonAnimator = nextPokemon.GetComponentInChildren<Animator>();
                playerPokemonStatsCalculator = nextPokemon.GetComponent<PokemonStatsCalculator>();

                nextPokemon.SetActive(true);
                nextPokemon.transform.localScale = new Vector3(1, 1, 1);
                nextPokemon.transform.LookAt(Vector3.forward + nextPokemon.transform.position);

                battleHUD.ActionSelector.SetActive(true);

                battleHUD.SetData(wildPokemonStatsCalculator, playerPokemonStatsCalculator);
                yield return battleDialogBox.TypeDialog($"Go {playerPokemonStatsCalculator.pokemonBase.Name}!");
            }
            else
            {
                playerManager.animator.SetBool("isInBattle", false);
                wildPokemonAnimator.SetBool("isInBattle", false);
                collisionManager.transform.gameObject.SetActive(true); // player trigger collider
                collisionManager.playerCollider.enabled = true; // main player collider
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
            battleHUD.ActionSelector.SetActive(true);
        }
    }

    IEnumerator RunMove(PokemonStatsCalculator sourceUnit, PokemonStatsCalculator targetUnit, Move move)
    {
        yield return battleDialogBox.TypeDialog($"{sourceUnit.pokemonBase.Name} used {move.Base.Name}");
        move.PP--;
        var damageDetails = targetUnit.TakeDamage(move.Base, sourceUnit);
        yield return battleHUD.UpdatePokemonHP(targetUnit,battleHUD.wildPokemonHPBar);
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return battleDialogBox.TypeDialog($"{targetUnit.pokemonBase.Name} Fainted");
            playerManager.animator.SetBool("isInBattle", false);
            targetUnit.gameObject.tag = "FaintedPokemon";
            targetUnit.GetComponentInChildren<Animator>().SetBool("isInBattle", false);
            targetUnit.GetComponentInChildren<Animator>().SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            sourceUnit.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            StartCoroutine(EnemyMove());
            battleHUD.SetMovesUI(sourceUnit.Moves);
        }
    }

    private void BattleOver(PokemonStatsCalculator faintedUnit)
    {
        if(faintedUnit.tag == "PartyPokemon")
        {
            var nextPokemon = pokemonPartyManager.GetHealthyPokemon();
            if (nextPokemon != null)
            {

            }
        }
    }

    public void OpenPartyScreen()
    {
        partyScreen.SetPartyData(pokemonPartyManager.pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return battleDialogBox.TypeDialog("A critical hit!");

        if(damageDetails.TypeEffectiveness > 1f)
            yield return battleDialogBox.TypeDialog("It's super effective");
        else if(damageDetails.TypeEffectiveness < 1f)
            yield return battleDialogBox.TypeDialog("It's not very effective");
    }
}