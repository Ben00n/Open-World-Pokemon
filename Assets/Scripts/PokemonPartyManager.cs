using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonPartyManager : MonoBehaviour
{
    public List<GameObject> pokemons;

    public GameObject GetHealthyPokemon()
    {
        return pokemons.Where(x => x.GetComponent<PokemonStatsCalculator>().currentHP > 0).FirstOrDefault();
    }
}
