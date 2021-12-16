using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUIManager : MonoBehaviour
{
    public PartyMemberUI[] memberSlots;

    private void Awake()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<GameObject> pokemons)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i].GetComponent<PokemonStatsCalculator>());
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }
    }
}
