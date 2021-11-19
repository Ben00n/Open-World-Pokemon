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

    #region Perform Player Move Buttons and IENumerator
    IEnumerator PerformPlayerMove(Move move)
    {
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

    public void PerformMove1()
    {
        var move = playerPokemonStatsCalculator.Moves[0];
        StartCoroutine(PerformPlayerMove(move));
    }


    public void PerformMove2()
    {
        var move = playerPokemonStatsCalculator.Moves[1];
        StartCoroutine(PerformPlayerMove(move));
    }

    public void PerformMove3()
    {
        var move = playerPokemonStatsCalculator.Moves[2];
        StartCoroutine(PerformPlayerMove(move));
    }

    public void PerformMove4()
    {
        var move = playerPokemonStatsCalculator.Moves[3];
        StartCoroutine(PerformPlayerMove(move));
    }
    #endregion
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
                OpenPartyScreen();
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
            else
            {

            }
        }
    }

    public void OpenPartyScreen()
    {
        partyScreen.SetPartyData(pokemonPartyManager.pokemons);
        if (partyScreen.gameObject.activeInHierarchy)
            partyScreen.gameObject.SetActive(false);
        else
            partyScreen.gameObject.SetActive(true);

    }

    #region Pokemon Switching Buttons Functions and IENumerator

    IEnumerator SwitchingPokemon(GameObject newPokemon)
    {
        if (playerPokemonStatsCalculator.currentHP > 0)
        {
            yield return battleDialogBox.TypeDialog($"Come back {playerPokemonStatsCalculator.pokemonBase.Name}");

            newPokemon.transform.position = playerPokemonStatsCalculator.transform.position;
            playerPokemonStatsCalculator.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);

            playerPokemonAnimatorManager = newPokemon.GetComponentInChildren<PokemonAnimatorManager>();
            playerPokemonManager = newPokemon.GetComponent<PokemonManager>();
            playerPokemonAnimator = newPokemon.GetComponentInChildren<Animator>();
            playerPokemonStatsCalculator = newPokemon.GetComponent<PokemonStatsCalculator>();

            newPokemon.SetActive(true);
            newPokemon.transform.localScale = new Vector3(1, 1, 1);
            newPokemon.transform.LookAt(Vector3.forward + newPokemon.transform.position);

            battleHUD.SetData(wildPokemonStatsCalculator, playerPokemonStatsCalculator);
            yield return battleDialogBox.TypeDialog($"Go {playerPokemonStatsCalculator.pokemonBase.Name}!");

            StartCoroutine(EnemyMove());
        }
        else //same logic without enemymove
        {
            yield return battleDialogBox.TypeDialog($"Come back {playerPokemonStatsCalculator.pokemonBase.Name}");

            newPokemon.transform.position = playerPokemonStatsCalculator.transform.position;
            playerPokemonStatsCalculator.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);

            playerPokemonAnimatorManager = newPokemon.GetComponentInChildren<PokemonAnimatorManager>();
            playerPokemonManager = newPokemon.GetComponent<PokemonManager>();
            playerPokemonAnimator = newPokemon.GetComponentInChildren<Animator>();
            playerPokemonStatsCalculator = newPokemon.GetComponent<PokemonStatsCalculator>();

            newPokemon.SetActive(true);
            newPokemon.transform.localScale = new Vector3(1, 1, 1);
            newPokemon.transform.LookAt(Vector3.forward + newPokemon.transform.position);

            battleHUD.ActionSelector.SetActive(true);

            battleHUD.SetData(wildPokemonStatsCalculator, playerPokemonStatsCalculator);
            yield return battleDialogBox.TypeDialog($"Go {playerPokemonStatsCalculator.pokemonBase.Name}!");
        }
    }

    public void SwitchPokemon1()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[0];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }

    public void SwitchPokemon2()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[1];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }

    public void SwitchPokemon3()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[2];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }

    public void SwitchPokemon4()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[3];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }

    public void SwitchPokemon5()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[4];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }

    public void SwitchPokemon6()
    {
        var selectedPokemon = pokemonPartyManager.pokemons[5];
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>().currentHP <= 0)
        {
            Debug.Log("Cant switch to deaded pokemon");
            return;
        }
        if (selectedPokemon.GetComponent<PokemonStatsCalculator>() == playerPokemonStatsCalculator)
        {
            Debug.Log("Cant switch to current pokemon");
            return;
        }
        battleHUD.ActionSelector.SetActive(false);
        StartCoroutine(SwitchingPokemon(selectedPokemon));
    }
    #endregion

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