using System.Collections;
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

    #region StatFormulas
    private int GetPokemonHP()
    {
        return Mathf.FloorToInt(((2 * pokemonBase.MaxHp) + 31 + (252 / 4)) * Level / 100f) + Level + 10;
    }

    private int GetPokemonAttack()
    {
        return Mathf.FloorToInt((0.01f*(2 * pokemonBase.Attack + 31 + (252 / 4)) * Level) + 5);
    }
    private int GetPokemonDefense()
    {
        return Mathf.FloorToInt((0.01f * (2 * pokemonBase.Defense + 31 + (252 / 4)) * Level) + 5);
    }
    private int GetPokemonSpAttack()
    {
        return Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpAttack + 31 + (252 / 4)) * Level) + 5);
    }
    private int GetPokemonSpDefense()
    {
        return Mathf.FloorToInt((0.01f * (2 * pokemonBase.SpDefense + 31 + (252 / 4)) * Level) + 5);
    }
    private int GetPokemonSpeed()
    {
        return Mathf.FloorToInt((0.01f * (2 * pokemonBase.Speed + 31 + (252 / 4)) * Level) + 5);
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

    private void Start()
    {
        maxHP = GetPokemonHP();
        currentHP = GetPokemonHP();
        currentAttack = GetPokemonAttack();
        currentDefense = GetPokemonDefense();
        currentSpAttack = GetPokemonSpAttack();
        currentSpDefense = GetPokemonSpDefense();
        currentSpeed = GetPokemonSpeed();
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
        if (moveBase.isSpecialAttack)
        {
            float d = a * moveBase.Power * ((float)attacker.currentSpAttack / currentSpDefense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            currentHP -= damage;
        }
        else if (moveBase.isPhysicalAttack)
        {
            float d = a * moveBase.Power * ((float)attacker.currentAttack / currentAttack) + 2;
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
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
