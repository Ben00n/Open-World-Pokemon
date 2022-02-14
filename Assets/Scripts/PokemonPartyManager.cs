using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonPartyManager : MonoBehaviour
{
    [Header("Party Pokemons")]
    [SerializeField] public List<GameObject> partyPokemons;

    [Header("PC Pokemons")]
    [SerializeField] public List<GameObject> pcPokemons;

    BattleDialogBox dialogBox;

    private void Awake()
    {
        dialogBox = FindObjectOfType<BattleDialogBox>();
    }

    public GameObject GetHealthyPokemon()
    {
        return partyPokemons.Where(x => x.GetComponent<PokemonStatsCalculator>().currentHP > 0).FirstOrDefault();
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var pokemon in partyPokemons)
        {
            var evo = pokemon.GetComponent<PokemonStatsCalculator>().CheckForEvolution();
            if (evo != null)
            {
                yield return dialogBox.TypeDialog($"{pokemon.GetComponent<PokemonStatsCalculator>().pokemonBase.Name} evolved into {evo.EvolvesInto.GetComponent<PokemonStatsCalculator>().pokemonBase.Name}!");
                pokemon.GetComponent<PokemonStatsCalculator>().Evolve(evo);
                break;
            }
        }
    }

    public void SwapPartyPokemon(int pokemonIndexA, int pokemonIndexB)
    {
        GameObject pokemon = partyPokemons[pokemonIndexA];
        partyPokemons[pokemonIndexA] = partyPokemons[pokemonIndexB];
        partyPokemons[pokemonIndexB] = pokemon;
    }

    public void SwapPcPokemons(int pokemonIndexA, int pokemonIndexB)
    {
        GameObject pokemon = pcPokemons[pokemonIndexA];
        pcPokemons[pokemonIndexA] = pcPokemons[pokemonIndexB];
        pcPokemons[pokemonIndexB] = pokemon;
    }
}
