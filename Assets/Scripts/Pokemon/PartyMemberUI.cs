using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    BattleHUD battleHUD;

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ExpBar expBar;
    [SerializeField] Image icon;
    [SerializeField] public int index;

    private void Awake()
    {
        battleHUD = FindObjectOfType<BattleHUD>();
    }

    public void SetData(PokemonStatsCalculator pokemon,int index)
    {
        nameText.text = pokemon.pokemonBase.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.currentHP / pokemon.maxHP);
        expBar?.SetEXP(GetNormalizedExp(pokemon));
        SetStatusText(pokemon);
        icon.sprite = pokemon.pokemonBase.GetSprite;
        this.index = index;
    }

    private void SetStatusText(PokemonStatsCalculator pokemon)
    {
        if (pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = pokemon.Status.Id.ToString().ToUpper();
            statusText.color = battleHUD.statusColors[pokemon.Status.Id];
        }
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
