using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    BattleHUD battleHUD;
    PokemonAnimatorManager pokemonAnimatorManager;
    PokemonPartyManager pokemonPartyManager;
    PokemonManager pokemonManager;
    public PokemonStatsCalculator pokemonStatsCalculator;
    public PokemonStatsCalculator wildPokemonStatsCalculator;
    public PokemonManager wildPokemonManager;
    public PokemonAnimatorManager wildPokemonAnimatorManager;

    private void Awake()
    {
        battleHUD = FindObjectOfType<BattleHUD>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
    }

    public void PerformMove1()
    {
        StartCoroutine(PerformPlayerMove1());
    }

    IEnumerator PerformPlayerMove1()
    {
        var move = pokemonStatsCalculator.pokemonBase.Move[0];
        if (move.isPhysicalAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            //do whateve
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

        yield return new WaitForSeconds(1);
    }

    public void PerformMove2()
    {
        StartCoroutine(PerformPlayerMove2());
    }

    IEnumerator PerformPlayerMove2()
    {
        var move = pokemonStatsCalculator.pokemonBase.Move[1];
        if (move.isPhysicalAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            //do whateve
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

        yield return new WaitForSeconds(1);
    }

    public void PerformMove3()
    {
        StartCoroutine(PerformPlayerMove3());
    }

    IEnumerator PerformPlayerMove3()
    {
        var move = pokemonStatsCalculator.pokemonBase.Move[2];
        if (move.isPhysicalAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            //do whateve
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

        yield return new WaitForSeconds(1);
    }

    public void PerformMove4()
    {
        StartCoroutine(PerformPlayerMove4());
    }

    IEnumerator PerformPlayerMove4()
    {
        var move = pokemonStatsCalculator.pokemonBase.Move[3];
        if (move.isPhysicalAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }

        if (move.isSpecialAttack)
        {
            pokemonManager.isAttacking = true;
            pokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }

        bool isFainted = wildPokemonStatsCalculator.TakeDamage(move, pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>());
        yield return battleHUD.UpdateWildPokemonHP();

        if (isFainted)
        {
            //do whateve
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

        yield return new WaitForSeconds(1);
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
        bool isFainted = pokemonStatsCalculator.TakeDamage(move, wildPokemonStatsCalculator);
        yield return battleHUD.UpdateMyPokemonHP();
        

        if (isFainted)
        {
            //i fainted -> do smthn
        }
        else
        {
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        if (pokemonPartyManager.pokemons.Count > 0)
        {
            pokemonManager = pokemonPartyManager.pokemons[0].GetComponent<PokemonManager>();
            pokemonStatsCalculator = pokemonPartyManager.pokemons[0].GetComponent<PokemonStatsCalculator>();
            pokemonAnimatorManager = pokemonPartyManager.pokemons[0].GetComponentInChildren<PokemonAnimatorManager>();
        }
    }
}