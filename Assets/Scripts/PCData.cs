using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCData
{
    public PCData(List<GameObject> pokemonList, int page)
    {
        PokemonList = pokemonList;
        Page = page;
    }

    public List<GameObject> PokemonList { get; } = null;
    public int Page { get; } = 1;
}
