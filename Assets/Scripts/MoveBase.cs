using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;
    [SerializeField] bool isSpecial;
    [SerializeField] bool isPhysical;
    [SerializeField] bool isStatus;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int maxPP;

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

    public bool isSpecialAttack
    {
        get { return isSpecial; }
    }

    public bool isPhysicalAttack
    {
        get { return isPhysical; }
    }

    public bool isStatusAttack
    {
        get { return isStatus; }
    }
}
