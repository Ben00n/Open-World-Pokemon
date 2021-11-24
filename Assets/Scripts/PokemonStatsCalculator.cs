﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PokemonStatsCalculator : MonoBehaviour
{
    PokemonPartyManager pokemonPartyManager;
    PokemonAnimatorManager pokemonAnimatorManager;
    PokemonManager pokemonManager;
    public PokemonBase pokemonBase;

    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    [SerializeField]
    public bool isWild;

    [SerializeField]
    private int myLevel;
    public int Level { get { return myLevel; } set { myLevel = value; } }

    [Header("CurrentStats")]
    public int maxHP;
    public int currentHP;
    public int currentAttack;
    public int currentDefense;
    public int currentSpAttack;
    public int currentSpDefense;
    public int currentSpeed;

    public int CurrentAttack { get { return GetStat(Stat.Attack); } }
    public int CurrentDefense { get { return GetStat(Stat.Defense); } }
    public int CurrentSpAttack { get { return GetStat(Stat.SpAttack); } }
    public int CurrentSpDefense { get { return GetStat(Stat.SpDefense); } }
    public int CurrentSpeed { get { return GetStat(Stat.Speed); } }

    #region StatFormulas
    public int GetPokemonHP()
    {
        return Mathf.FloorToInt(((2 * pokemonBase.MaxHp) + 31 + (252 / 4)) * Level / 100f) + Level + 10;
    }

    #endregion

    private void SetPokemonLevel()
    {
        Level = Random.Range(1, 50);
    }
    private void SetPokemonMoves()
    {
        Moves = new List<Move>();
        foreach (var move in pokemonBase.LearnableMoves)
        {
            if (move.Level <= myLevel)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }
    }

    private void Awake()
    {
        pokemonAnimatorManager = GetComponentInChildren<PokemonAnimatorManager>();
        pokemonManager = GetComponent<PokemonManager>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        if (Level == 0 && isWild)
        {
            SetPokemonLevel();
            SetPokemonMoves();
        }
        else
        {
            foreach (var pokemon in pokemonPartyManager.pokemons)
            {
                Level = pokemon.GetComponent<PokemonStatsCalculator>().myLevel;
            }
 
        }
    }

    private void Update()
    {
        currentAttack = CurrentAttack;
        currentDefense = CurrentDefense;
        currentSpAttack = CurrentSpAttack;
        currentSpDefense = CurrentSpDefense;
        currentSpeed = CurrentSpeed;
    }

    private void Start()
    {
        CalculateStats();

        ResetStatBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Attack + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.Defense, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Defense + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpAttack + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpDefense + 31 + (252 / 4)) * Level) + 5));
        Stats.Add(Stat.Speed, Mathf.FloorToInt((0.01f * (2 * pokemonBase.Speed + 31 + (252 / 4)) * Level) + 5));

        maxHP = Mathf.FloorToInt(((2 * pokemonBase.MaxHp) + 31 + (252 / 4)) * Level / 100f) + Level + 10;
        currentHP = maxHP;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{pokemonBase.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{pokemonBase.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been modified to {StatBoosts[stat]}");
        }
    }

    public DamageDetails TakeDamage(MoveBase moveBase, PokemonStatsCalculator attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(moveBase.Type, this.pokemonBase.GetType1) * TypeChart.GetEffectiveness(moveBase.Type, this.pokemonBase.GetType2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        if (moveBase.Category == MoveCategory.Special)
        {
            float d = a * moveBase.Power * ((float)attacker.CurrentSpAttack / CurrentSpDefense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            currentHP -= damage;
        }
        else if (moveBase.Category == MoveCategory.Physical)
        {
            float d = a * moveBase.Power * ((float)attacker.CurrentAttack / CurrentDefense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            currentHP -= damage;
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            pokemonAnimatorManager.PlayTargetAnimation("Faint");
            damageDetails.Fainted = true;
        }
        else
        {
            pokemonAnimatorManager.PlayTargetAnimation("Hit");
            return damageDetails;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
