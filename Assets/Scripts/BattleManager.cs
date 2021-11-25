using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Button fightButton;

    private void Awake()
    {
        collisionManager = FindObjectOfType<CollisionManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        battleDialogBox = FindObjectOfType<BattleDialogBox>();
        battleHUD = FindObjectOfType<BattleHUD>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
    }

    public void ChooseFirstTurn()
    {
        if (playerPokemonStatsCalculator.CurrentSpeed >= wildPokemonStatsCalculator.CurrentSpeed)
            battleHUD.ActionSelector.SetActive(true);
        else
        {
            battleHUD.ActionSelector.SetActive(false);
            StartCoroutine(EnemyMove());
        }
    }

    #region Perform Player Move Buttons and IENumerator
    IEnumerator PerformPlayerMove(Move move)
    {
        bool canRunMove = playerPokemonStatsCalculator.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(playerPokemonStatsCalculator);
            yield return battleHUD.UpdatePokemonHP(playerPokemonStatsCalculator, battleHUD.playerPokemonHPBar);
            yield return new WaitForSeconds(1);
            StartCoroutine(EnemyMove());
            yield break;
        }

        if (move.Base.Category == MoveCategory.Physical)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
        }
        if (move.Base.Category == MoveCategory.Special)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
        }
        if (move.Base.Category == MoveCategory.Status)
        {
            playerPokemonManager.isAttacking = true;
            playerPokemonAnimatorManager.PlayTargetAnimation("Status");
        }

        yield return RunMove(playerPokemonStatsCalculator, wildPokemonStatsCalculator, move);
        battleHUD.SetMovesUI(playerPokemonStatsCalculator.Moves);

        StartCoroutine(EnemyMove());
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
        if(wildPokemonStatsCalculator.currentHP > 0)
        {
            bool canRunMove = wildPokemonStatsCalculator.OnBeforeTurn();
            if (!canRunMove)
            {
                yield return ShowStatusChanges(wildPokemonStatsCalculator);
                yield return battleHUD.UpdatePokemonHP(wildPokemonStatsCalculator, battleHUD.wildPokemonHPBar);
                yield return new WaitForSeconds(1);
                battleHUD.ActionSelector.SetActive(true);
                yield break;
            }

            var move = wildPokemonStatsCalculator.GetRandomMove();
            if (move.Base.Category == MoveCategory.Special)
            {
                wildPokemonManager.isAttacking = true;
                wildPokemonAnimatorManager.PlayTargetAnimation("Special Attack");
            }
            if (move.Base.Category == MoveCategory.Physical)
            {
                wildPokemonManager.isAttacking = true;
                wildPokemonAnimatorManager.PlayTargetAnimation("Physical Attack");
            }
            if (move.Base.Category == MoveCategory.Status)
            {
                wildPokemonManager.isAttacking = true;
                wildPokemonAnimatorManager.PlayTargetAnimation("Status");
            }

            yield return RunMove(wildPokemonStatsCalculator, playerPokemonStatsCalculator, move);
            yield return new WaitForSeconds(1);
            battleHUD.ActionSelector.SetActive(true);

        }
    }

    IEnumerator RunMove(PokemonStatsCalculator sourceUnit, PokemonStatsCalculator targetUnit, Move move)
    {
        yield return ShowStatusChanges(sourceUnit);

        move.PP--;
        yield return battleDialogBox.TypeDialog($"{sourceUnit.pokemonBase.Name} used {move.Base.Name}");

        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit, targetUnit);
        }
        else
        {
            var damageDetails = targetUnit.TakeDamage(move.Base, sourceUnit);
            if (targetUnit.tag == "Pokemon")
            {
                yield return battleHUD.UpdatePokemonHP(targetUnit, battleHUD.wildPokemonHPBar);
            }
            else if (targetUnit.tag == "PartyPokemon")
            {
                yield return battleHUD.UpdatePokemonHP(targetUnit, battleHUD.playerPokemonHPBar);
            }
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.currentHP <= 0)
        {
            yield return BattleOver(targetUnit);
        }

        //Statuses like burn or poison will hurt pokemon AFTER turn
        sourceUnit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit);
        if (targetUnit.tag == "PartyPokemon")
        {
            yield return battleHUD.UpdatePokemonHP(sourceUnit, battleHUD.wildPokemonHPBar);
        }
        if (targetUnit.tag == "Pokemon")
        {
            yield return battleHUD.UpdatePokemonHP(sourceUnit, battleHUD.playerPokemonHPBar);
        }
        if (sourceUnit.currentHP <= 0)
        {
            yield return BattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(Move move, PokemonStatsCalculator source,PokemonStatsCalculator target)
    {
        var effects = move.Base.Effects;
        // Stat boosting
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator BattleOver(PokemonStatsCalculator faintedUnit)
    {
        if (faintedUnit.tag == "PartyPokemon")
        {
            var nextPokemon = pokemonPartyManager.GetHealthyPokemon();
            faintedUnit.gameObject.SetActive(false);
            if (nextPokemon != null)
            {
                OpenPartyScreen();
                fightButton.interactable = false;
                pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
            }
            else
            {
                yield return battleDialogBox.TypeDialog($"{faintedUnit.pokemonBase.Name} Fainted");
                playerManager.animator.SetBool("isInBattle", false);
                wildPokemonAnimator.SetBool("isInBattle", false);
                collisionManager.transform.gameObject.SetActive(true); // player trigger collider
                collisionManager.playerCollider.enabled = true; // main player collider
                pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
            }
        }
        else if (faintedUnit.tag == "Pokemon")
        {
            yield return battleDialogBox.TypeDialog($"{faintedUnit.pokemonBase.Name} Fainted");
            playerManager.animator.SetBool("isInBattle", false);
            playerPokemonStatsCalculator.gameObject.SetActive(false);
            faintedUnit.gameObject.tag = "FaintedPokemon";
            faintedUnit.GetComponentInChildren<Animator>().SetBool("isInBattle", false);
            faintedUnit.GetComponentInChildren<Animator>().SetBool("isFainted", true);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
        }
    }

    IEnumerator ShowStatusChanges(PokemonStatsCalculator pokemon)
    {
        while(pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return battleDialogBox.TypeDialog(message);
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

            pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());

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

            battleHUD.SetData(wildPokemonStatsCalculator, playerPokemonStatsCalculator);
            yield return battleDialogBox.TypeDialog($"Go {playerPokemonStatsCalculator.pokemonBase.Name}!");

            ChooseFirstTurn();

            pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
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