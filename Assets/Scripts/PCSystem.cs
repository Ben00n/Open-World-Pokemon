using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCSystem : MonoBehaviour
{
    [SerializeField] private GameObject pokemonButtonPrefab = null;
    [SerializeField] private Transform pokemonButtonHolderTransform = null;

    private PCData pcData = null;

    public void SetPcData(PCData pcData)
    {
        ClearPokemonButtons();
        this.pcData = pcData;

        var pokemonList = pcData.PokemonList;
        for (int i = 0; i < pokemonList.Count; i++)
        {
            GameObject buttonInstance = Instantiate(pokemonButtonPrefab, pokemonButtonHolderTransform);
            var pokemon = pokemonList[i].GetComponent<PokemonStatsCalculator>();
            buttonInstance.GetComponent<PCPokemonButton>().Initialise(this, pokemon);

        }
    }

    private void ClearPokemonButtons()
    {
        foreach (Transform child in pokemonButtonHolderTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
