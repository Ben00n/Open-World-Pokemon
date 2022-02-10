using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour, IInteractable
{
    private PCData pcData;

    public void Interact(GameObject other)
    {
        var pcPokemonList = other.GetComponent<PokemonPartyManager>().pcPokemons;
        pcData = new PCData(pcPokemonList, 1);
    }
}
