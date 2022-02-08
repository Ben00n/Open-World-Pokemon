using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonPartyManager : MonoBehaviour
{
    public List<GameObject> pokemons;
    BattleDialogBox dialogBox;

    private void Awake()
    {
        dialogBox = FindObjectOfType<BattleDialogBox>();
    }

    public GameObject GetHealthyPokemon()
    {
        return pokemons.Where(x => x.GetComponent<PokemonStatsCalculator>().currentHP > 0).FirstOrDefault();
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var pokemon in pokemons)
        {
            var evo = pokemon.GetComponent<PokemonStatsCalculator>().CheckForEvolution();
            if (evo != null)
            {
                yield return dialogBox.TypeDialog($"{pokemon.GetComponent<PokemonStatsCalculator>().pokemonBase.Name} evolved into {evo.EvolvesInto.GetComponent<PokemonStatsCalculator>().pokemonBase.Name}");
                pokemon.GetComponent<PokemonStatsCalculator>().Evolve(evo);
            }
        }
    }

    public void SwapPokemon(int pokemonIndexA, int pokemonIndexB)
    {
        GameObject pokemon = pokemons[pokemonIndexA];
        pokemons[pokemonIndexA] = pokemons[pokemonIndexB];
        pokemons[pokemonIndexB] = pokemon;
    }
}
