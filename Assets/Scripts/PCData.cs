using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCData
{
    public PCData(List<GameObject> pokemonList)
    {
        PokemonList = pokemonList;
    }

    public List<GameObject> PokemonList { get; } = null;
}
