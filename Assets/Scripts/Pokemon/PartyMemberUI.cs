using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image icon;
    [SerializeField] public int index;

    public void SetData(PokemonStatsCalculator pokemon,int index)
    {
        nameText.text = pokemon.pokemonBase.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.currentHP / pokemon.maxHP);
        icon.sprite = pokemon.pokemonBase.GetSprite;
        this.index = index;
    }

    public int GetPokemonIndex()
    {
        return index;
    }
}
