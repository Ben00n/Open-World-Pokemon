using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PokemonPartyManager : MonoBehaviour
{
    public List<GameObject> pokemons;
    BattleDialogBox dialogBox;
    BattleHUD battleHUD;

    private void Awake()
    {
        dialogBox = FindObjectOfType<BattleDialogBox>();
        battleHUD = FindObjectOfType<BattleHUD>();
    }

    public GameObject GetHealthyPokemon()
    {
        return pokemons.Where(x => x.GetComponent<PokemonStatsCalculator>().currentHP > 0).FirstOrDefault();
    }

    public IEnumerator CheckForEvolutions()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            var evo = pokemons[i].GetComponent<PokemonStatsCalculator>().CheckForEvolution();
            if(evo != null)
            {
                yield return dialogBox.TypeDialog($"{pokemons[i].GetComponent<PokemonStatsCalculator>().pokemonBase.Name} evolved into {evo.EvolvesInto.GetComponent<PokemonStatsCalculator>().pokemonBase.Name}");
                pokemons[i].GetComponent<PokemonStatsCalculator>().Evolve(evo);
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
