using System.Collections;
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

    [Header("Player Pokemon Components")]
    public PokemonStatsCalculator playerPokemonStatsCalculator;
    public PokemonAnimatorManager playerPokemonAnimatorManager;
    public PokemonManager playerPokemonManager;

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
        var move = playerPokemonStatsCalculator.pokemonBase.Move[0];
        if (move.isPhysicalAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} used {move.Name}");

        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} Fainted");
            yield return new WaitForSeconds(1f);
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonManager.gameObject.tag = "FaintedPokemon";
            wildPokemonAnimator.SetBool("isInBattle", false);
            wildPokemonAnimator.SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    public void PerformMove2()
    {
        StartCoroutine(PerformPlayerMove2());
    }

    IEnumerator PerformPlayerMove2()
    {
        var move = playerPokemonStatsCalculator.pokemonBase.Move[1];
        if (move.isPhysicalAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} used {move.Name}");


        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} Fainted");
            yield return new WaitForSeconds(1f);
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonManager.gameObject.tag = "FaintedPokemon";
            wildPokemonAnimator.SetBool("isInBattle", false);
            wildPokemonAnimator.SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    public void PerformMove3()
    {
        StartCoroutine(PerformPlayerMove3());
    }

    IEnumerator PerformPlayerMove3()
    {
        var move = playerPokemonStatsCalculator.pokemonBase.Move[2];
        if (move.isPhysicalAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} used {move.Name}");


        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} Fainted");
            yield return new WaitForSeconds(1f);
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonManager.gameObject.tag = "FaintedPokemon";
            wildPokemonAnimator.SetBool("isInBattle", false);
            wildPokemonAnimator.SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    public void PerformMove4()
    {
        StartCoroutine(PerformPlayerMove4());
    }

    IEnumerator PerformPlayerMove4()
    {
        var move = playerPokemonStatsCalculator.pokemonBase.Move[3];
        if (move.isPhysicalAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} used {move.Name}");


        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} Fainted");
            yield return new WaitForSeconds(1f);
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonManager.gameObject.tag = "FaintedPokemon";
            wildPokemonAnimator.SetBool("isInBattle", false);
            wildPokemonAnimator.SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        var move = wildPokemonStatsCalculator.GetRandomMove();
        if (move.isSpecialAttack)
        {
            wildPokemonManager.isAttacking = true;
            wildPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }
        if(move.isPhysicalAttack)
        {
            wildPokemonManager.isAttacking = true;
            wildPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }
        yield return battleDialogBox.TypeDialog($"{wildPokemonStatsCalculator.pokemonBase.Name} used {move.Name}");

        bool isFainted = playerPokemonStatsCalculator.TakeDamage(move, wildPokemonStatsCalculator);
        yield return battleHUD.UpdateMyPokemonHP();
        

        if (isFainted)
        {
            yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} Fainted");
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonAnimator.SetBool("isInBattle", false);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            playerPokemonManager.transform.gameObject.SetActive(false); // party pokemon gameobject disappear
        }
        else
        {
            yield return new WaitForSeconds(1);
            battleHUD.ActionSelector.SetActive(true);
        }
    }

    private void Update()
    {
        if (pokemonPartyManager.pokemons.Count > 0)
        {
            playerPokemonManager = pokemonPartyManager.pokemons[0].GetComponent<PokemonManager>();
            playerPokemonStatsCalculator = pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>();
            playerPokemonAnimatorManager = pokemonPartyManager.pokemons[0].GetComponentInChildren<PokemonAnimatorManager>();
        }
    }
}