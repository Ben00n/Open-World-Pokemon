using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour, IInteractable
{
    [SerializeField] private PcDataEvent onStartPcScenario = null;
    public void Interact(GameObject other)
    {
        Debug.Log("Interacted with PC");
        var pcPokemonList = other.GetComponent<PokemonPartyManager>().pcPokemons;
        PCData pcData = new PCData(pcPokemonList);

        onStartPcScenario.Raise(pcData);
    }
}
