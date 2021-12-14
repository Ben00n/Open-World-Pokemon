using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction { Move, SwitchPokemon, UseItem, Run}
public class BattleManager : MonoBehaviour
{
    CollisionManager collisionManager;
    PlayerManager playerManager;
    BattleDialogBox battleDialogBox;
    BattleHUD battleHUD;
    PokemonPartyManager pokemonPartyManager;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveDecideUI moveDecideUI;

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

    public int escapeAttempts;
    MoveBase moveToLearn;

    private void Awake()
    {
        collisionManager = FindObjectOfType<CollisionManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        battleDialogBox = FindObjectOfType<BattleDialogBox>();
        battleHUD = FindObjectOfType<BattleHUD>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
    }

    IEnumerator RunTurns(BattleAction playerAction, Move move)
    {
        if(playerAction == BattleAction.Move)
        {
            playerPokemonStatsCalculator.CurrentMove = move;
            wildPokemonStatsCalculator.CurrentMove = wildPokemonStatsCalculator.GetRandomMove();

            int playerMovePriority = playerPokemonStatsCalculator.CurrentMove.Base.Priority;
            int enemyMovePriority = wildPokemonStatsCalculator.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerPokemonStatsCalculator.CurrentSpeed >= wildPokemonStatsCalculator.CurrentSpeed;

            var firstUnit = (playerGoesFirst) ? playerPokemonStatsCalculator : wildPokemonStatsCalculator;
            var secondUnit = (playerGoesFirst) ? wildPokemonStatsCalculator : playerPokemonStatsCalculator;

            //First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.CurrentMove);
            yield return RunAfterTurn(firstUnit, secondUnit);
            if (secondUnit.currentHP <= 0) yield break;

            //Second Turn
            yield return RunMove(secondUnit, firstUnit, secondUnit.CurrentMove);
            yield return RunAfterTurn(secondUnit, firstUnit);
            if (firstUnit.currentHP <= 0) yield break;
            battleHUD.ActionSelector.SetActive(true);
        }
        else
        {
            if(playerAction == BattleAction.SwitchPokemon)
            {

            }

            else if(playerAction == BattleAction.Run)
            {

            }
            //Enemy Turn
            var enemyMove = wildPokemonStatsCalculator.GetRandomMove();
            yield return RunMove(wildPokemonStatsCalculator,playerPokemonStatsCalculator,enemyMove);
            yield return RunAfterTurn(wildPokemonStatsCalculator, playerPokemonStatsCalculator);
            if (playerPokemonStatsCalculator.currentHP <= 0) yield break;
            battleHUD.ActionSelector.SetActive(true);
        }
    }

    IEnumerator RunAfterTurn(PokemonStatsCalculator sourceUnit,PokemonStatsCalculator targetUnit)
    {
        if (sourceUnit.currentHP <= 0 || targetUnit.currentHP <= 0) yield break;

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

    #region Perform Player Move Buttons

    public void PerformMove1()
    {
        var move = playerPokemonStatsCalculator.Moves[0];
        if (move.PP == 0)
        {
            battleHUD.ActionSelector.SetActive(true);
            return;
        }
        StartCoroutine(RunTurns(BattleAction.Move, move));
    }


    public void PerformMove2()
    {
        var move = playerPokemonStatsCalculator.Moves[1];
        if (move.PP == 0)
        {
            battleHUD.ActionSelector.SetActive(true);
            return;
        }
        StartCoroutine(RunTurns(BattleAction.Move, move));
    }

    public void PerformMove3()
    {
        var move = playerPokemonStatsCalculator.Moves[2];
        if (move.PP == 0)
        {
            battleHUD.ActionSelector.SetActive(true);
            return;
        }
        StartCoroutine(RunTurns(BattleAction.Move, move));
    }

    public void PerformMove4()
    {
        var move = playerPokemonStatsCalculator.Moves[3];
        if (move.PP == 0)
        {
            battleHUD.ActionSelector.SetActive(true);
            return;
        }
        StartCoroutine(RunTurns(BattleAction.Move, move));
    }

    #endregion

    private void AnimationDecidor(PokemonStatsCalculator pokemon,Move move)
    {
        if (move.Base.Category == MoveCategory.Physical)
        {
            pokemon.GetComponent<PokemonManager>().isAttacking = true;
            pokemon.GetComponentInChildren<PokemonAnimatorManager>().PlayTargetAnimation("Physical Attack");
        }
        if (move.Base.Category == MoveCategory.Special)
        {
            pokemon.GetComponent<PokemonManager>().isAttacking = true;
            pokemon.GetComponentInChildren<PokemonAnimatorManager>().PlayTargetAnimation("Special Attack");
        }
        if (move.Base.Category == MoveCategory.Status)
        {
            pokemon.GetComponent<PokemonManager>().isAttacking = true;
            pokemon.GetComponentInChildren<PokemonAnimatorManager>().PlayTargetAnimation("Status");
        }
    }

    IEnumerator RunMove(PokemonStatsCalculator sourceUnit, PokemonStatsCalculator targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit);
            if (sourceUnit.tag == "Pokemon")
                yield return battleHUD.UpdatePokemonHP(sourceUnit, battleHUD.wildPokemonHPBar);
            else if (sourceUnit.tag == "PartyPokemon")
                yield return battleHUD.UpdatePokemonHP(sourceUnit, battleHUD.playerPokemonHPBar);

            yield break;
        }
        yield return ShowStatusChanges(sourceUnit);

        move.PP--;
        battleHUD.SetMovesUI(playerPokemonStatsCalculator.Moves);
        AnimationDecidor(sourceUnit, move);
        yield return battleDialogBox.TypeDialog($"{sourceUnit.pokemonBase.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit, targetUnit))
        {
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit, targetUnit,move.Base.Target);
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

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.currentHP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit, targetUnit, secondary.Target);
                }
            }


            if (targetUnit.currentHP <= 0)
            {
                yield return BattleOver(targetUnit);
            }
        }
        else
        {
            yield return battleDialogBox.TypeDialog($"{sourceUnit.pokemonBase.Name}'s attack missed");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, PokemonStatsCalculator source,PokemonStatsCalculator target, MoveTarget moveTarget)
    {
        // Stat boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
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
            yield return HandleFaintedExperience(faintedUnit); //NEED TO ADD IT AGAIN IN POISION DEATH THING
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

    IEnumerator HandleFaintedExperience(PokemonStatsCalculator faintedUnit)
    {
        //Exp Gain
        int expYield = faintedUnit.pokemonBase.ExpYield;
        int enemyLevel = faintedUnit.Level;

        int expGain = Mathf.FloorToInt((expYield * enemyLevel) / 7);
        playerPokemonStatsCalculator.Exp += expGain;
        yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} has gained {expGain} exp");
        yield return battleHUD.SetExpSmooth();

        //Check Level Up
        while (playerPokemonStatsCalculator.CheckForLevelUp())
        {
            battleHUD.SetLevel();
            playerPokemonStatsCalculator.CalculateStats();
            yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} has grew to level {playerPokemonStatsCalculator.Level}!");

            //Try to learn a new move
            var newMove = playerPokemonStatsCalculator.GetLearnableMoveAtCurrLevel();
            if(newMove != null)
            {
                if(playerPokemonStatsCalculator.Moves.Count < 4)
                {
                    playerPokemonStatsCalculator.LearnMove(newMove);
                    yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} has learned {newMove.Base.Name}");
                    battleHUD.SetMovesUI(playerPokemonStatsCalculator.Moves);
                }
                else
                {
                    yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} is trying to learn {newMove.Base.Name}");
                    yield return battleDialogBox.TypeDialog($"But it can not learn more than 4 moves!");
                    yield return ChooseMoveToForget(playerPokemonStatsCalculator, newMove.Base);
                    yield return new WaitUntil(() => !moveDecideUI.gameObject.activeInHierarchy);
                    yield return new WaitForSeconds(1f);
                }
            }

            yield return battleHUD.SetExpSmooth(true);
        }
        yield return new WaitForSeconds(1f);
    }

    IEnumerator ChooseMoveToForget(PokemonStatsCalculator pokemon, MoveBase newMove)
    {
        yield return battleDialogBox.TypeDialog($"Choose a move you want to forget");
        moveDecideUI.gameObject.SetActive(true);
        moveDecideUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
    }

    #region ChangeMovesOnLevelUp
    public void ChangeMoveNew()
    {
        StartCoroutine(battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} did not learn {moveToLearn.Name}"));
    }
    public void ChangeMove1()
    {
        var selectedMove = playerPokemonStatsCalculator.Moves[0].Base;
        StartCoroutine(battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!"));
        playerPokemonStatsCalculator.Moves[0] = new Move(moveToLearn);
        moveToLearn = null;
    }
    public void ChangeMove2()
    {
        var selectedMove = playerPokemonStatsCalculator.Moves[1].Base;
        StartCoroutine(battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!"));
        playerPokemonStatsCalculator.Moves[1] = new Move(moveToLearn);
        moveToLearn = null;
    }
    public void ChangeMove3()
    {
        var selectedMove = playerPokemonStatsCalculator.Moves[2].Base;
        StartCoroutine(battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!"));
        playerPokemonStatsCalculator.Moves[2] = new Move(moveToLearn);
        moveToLearn = null;
    }
    public void ChangeMove4()
    {
        var selectedMove = playerPokemonStatsCalculator.Moves[3].Base;
        StartCoroutine(battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!"));
        playerPokemonStatsCalculator.Moves[3] = new Move(moveToLearn);
        moveToLearn = null;
    }
    #endregion

    bool CheckIfMoveHits(Move move,PokemonStatsCalculator source, PokemonStatsCalculator target)
    {
        if (move.Base.AlwaysHits)
            return true;
        
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];



        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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

    IEnumerator TryToEscape()
    {
        escapeAttempts++;
        int playerSpeed = playerPokemonStatsCalculator.CurrentSpeed;
        int wildSpeed = wildPokemonStatsCalculator.CurrentSpeed;

        if(wildSpeed < playerSpeed)
        {
            yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} got away safely.");
            playerManager.animator.SetBool("isInBattle", false);
            wildPokemonStatsCalculator.GetComponentInChildren<Animator>().SetBool("isInBattle", false);
            playerPokemonStatsCalculator.gameObject.SetActive(false);
            collisionManager.transform.gameObject.SetActive(true); // player trigger collider
            collisionManager.playerCollider.enabled = true; // main player collider
            pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
        }
        else
        {
            float f = (playerSpeed * 128) / wildSpeed + 30 * escapeAttempts;
            f = f % 256;
            if(UnityEngine.Random.Range(0,256) < f)
            {
                yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} got away safely.");
                playerManager.animator.SetBool("isInBattle", false);
                wildPokemonStatsCalculator.GetComponentInChildren<Animator>().SetBool("isInBattle", false);
                playerPokemonStatsCalculator.gameObject.SetActive(false);
                collisionManager.transform.gameObject.SetActive(true); // player trigger collider
                collisionManager.playerCollider.enabled = true; // main player collider
                pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
            }
            else
            {
                yield return battleDialogBox.TypeDialog($"{playerPokemonStatsCalculator.pokemonBase.Name} has failed to escape.");
                StartCoroutine(RunTurns(BattleAction.Run, null));
            }
        }
    }

    public void RunFromBattle()
    {
        StartCoroutine(TryToEscape());
        battleHUD.ActionSelector.SetActive(false);
    }

    #region Pokemon Switching Buttons Functions and IENumerator

    IEnumerator SwitchingPokemon(GameObject newPokemon)
    {
        if(playerPokemonStatsCalculator.currentHP > 0)
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
            StartCoroutine(RunTurns(BattleAction.SwitchPokemon, null));
        }
        else
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

            battleHUD.ActionSelector.SetActive(true);

            pokemonPartyManager.pokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().OnBattleOver());
            battleHUD.ActionSelector.SetActive(true);
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
        else if(damageDetails.TypeEffectiveness < 1f && damageDetails.TypeEffectiveness > 0f)
            yield return battleDialogBox.TypeDialog("It's not very effective");
        else if(damageDetails.TypeEffectiveness == 0f)
            yield return battleDialogBox.TypeDialog("It had no effect at all");
    }
}