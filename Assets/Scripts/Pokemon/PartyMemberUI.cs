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
    [SerializeField] ExpBar expBar;
    [SerializeField] Image icon;
    [SerializeField] public int index;

    public void SetData(PokemonStatsCalculator pokemon,int index)
    {
        nameText.text = pokemon.pokemonBase.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.currentHP / pokemon.maxHP);
        expBar.SetEXP(GetNormalizedExp(pokemon));
        icon.sprite = pokemon.pokemonBase.GetSprite;
        this.index = index;
    }

    float GetNormalizedExp(PokemonStatsCalculator thisPokemon)
    {
        int currLevelExp = thisPokemon.pokemonBase.GetExpForLevel(thisPokemon.Level);
        int nextLevelExp = thisPokemon.pokemonBase.GetExpForLevel(thisPokemon.Level + 1);

        float normalizedExp = (float)(thisPokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public int GetPokemonIndex()
    {
        return index;
    }
}
