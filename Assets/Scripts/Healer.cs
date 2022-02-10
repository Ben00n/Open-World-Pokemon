using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour, IOccupation
{
    public string Name => "Heal";

    public void Heal(GameObject player)
    {
        var playerParty = player.GetComponent<PokemonPartyManager>();
        playerParty.partyPokemons.ForEach(p => p.GetComponent<PokemonStatsCalculator>().Heal());
    }

    public void Trigger(GameObject other)
    {
        var player = other.gameObject;
        Heal(player);
    }
}
