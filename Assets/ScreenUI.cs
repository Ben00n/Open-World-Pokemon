using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    PokemonPartyManager pokemonPartyManager;
    PartyMemberUI[] memberSlots;

    private void Awake()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
    }

    public void SetPartyData(List<GameObject> pokemons)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i].GetComponent<PokemonStatsCalculator>(), i);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        SetPartyData(pokemonPartyManager.partyPokemons);
    }
}
