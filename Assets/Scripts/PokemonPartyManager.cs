using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonPartyManager : MonoBehaviour
{
    public Transform player;
    PlayerManager playerManager;
    public List<GameObject> pokemons;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    //Instantiate(pokemon, player.transform.position, Quaternion.identity);

}
