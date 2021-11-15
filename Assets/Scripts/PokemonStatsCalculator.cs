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

    private float targetScale = 0.01f;
    private float shrinkSpeed = 3f;
    public bool shrinking = false;

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

    private void Awake()
    {
        pokemonAnimatorManager = GetComponentInChildren<PokemonAnimatorManager>();
        pokemonManager = GetComponent<PokemonManager>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        if (Level == 0 && isWild)
        {
            SetPokemonLevel();
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

    private void Update()
    {
        if (shrinking) // called in pokeballcollider
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * shrinkSpeed);
        }
    }

    public bool TakeDamage(MoveBase moveBase, PokemonStatsCalculator attacker)
    {
        if (moveBase.isSpecialAttack)
        {
            float modifiers = Random.Range(0.85f, 1f);
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * moveBase.Power * ((float)attacker.currentSpAttack / currentSpDefense) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            currentHP -= damage;
        }
        else if (moveBase.isPhysicalAttack)
        {
            float modifiers = Random.Range(0.85f, 1f);
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * moveBase.Power * ((float)attacker.currentAttack / currentAttack) + 2;
            int damage = Mathf.FloorToInt(d * modifiers);
            currentHP -= damage;
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            pokemonAnimatorManager.PlayTargetAnimation("Faint");
            return true;
        }
        return false;
    }

    public MoveBase GetRandomMove()
    {
        int r = Random.Range(0, pokemonBase.Move.Count);
        return pokemonBase.Move[r];
    }
}
