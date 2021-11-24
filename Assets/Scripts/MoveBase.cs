using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int maxPP;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int maximumPP
    {
        get { return maxPP; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical,Special,Status
}

public enum MoveTarget
{
    Foe,Self
}