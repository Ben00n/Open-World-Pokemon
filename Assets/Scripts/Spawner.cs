using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> pokemonList;
    public int xPos;
    public int zPos;
    public int pulledPokemon;
    public int pokemonCount;

    private void Start()
    {
        StartCoroutine(PokemonDrop());
    }

    IEnumerator PokemonDrop()
    {
        while (pokemonCount < 30)
        {
            pulledPokemon = Random.Range(0, pokemonList.Count);
            xPos = Random.Range(0, 100);
            zPos = Random.Range(0, 100);
            pokemonList[pulledPokemon].GetComponent<PokemonStatsCalculator>().isWild = true;
            pokemonList[pulledPokemon].tag = "Pokemon";
            Instantiate(pokemonList[pulledPokemon], new Vector3(xPos, 0, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            pokemonCount += 1;
        }
    }
}
