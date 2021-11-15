using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] int pokemonID;
    [SerializeField] string pokemonName;

    [SerializeField] GameObject prefab;
    [SerializeField] Sprite sprite;

    [Header("Pokemon Types")]
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [Header("Base Stats")] 
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] public List<MoveBase> Move;

    public string Name
    {
        get { return name; }
    }

    public GameObject getPrefab
    {
        get { return prefab; }
    }

    public Sprite getSprite
    {
        get { return sprite; }
    }

    public PokemonType GetType1
    {
        get { return type1; }
    }
    public PokemonType GetType2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int GetPokemonByID
    {
        get { return pokemonID; }
    }
}


public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Dark,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Steel,
    Ghost,
    Dragon,
    Fairy
}