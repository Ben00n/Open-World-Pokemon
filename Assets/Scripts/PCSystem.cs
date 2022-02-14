using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCSystem : MonoBehaviour
{

    public PCData pcData = null;

    PCPokemonButton[] pcPokemonButtons;

    private void Awake()
    {
        pcPokemonButtons = GetComponentsInChildren<PCPokemonButton>(true);
    }

    public void SetPcData(PCData pcData)
    {
        this.pcData = pcData;

        var pokemonList = pcData.PokemonList;
        for (int i = 0; i < pokemonList.Count; i++)
        {
            if(i < pokemonList.Count)
            {
                pcPokemonButtons[i].gameObject.SetActive(true);
                var pokemon = pokemonList[i].GetComponent<PokemonStatsCalculator>();
                pcPokemonButtons[i].Initialise(this, pokemon,i);
            }
            else
                pcPokemonButtons[i].gameObject.SetActive(false);
        }
    }
}
