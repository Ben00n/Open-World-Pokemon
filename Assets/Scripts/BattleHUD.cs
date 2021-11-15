using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    BattleManager battleManager;

    [Header("Wild Pokemon HUD")]
    [SerializeField] Text wildPokemonNameText;
    [SerializeField] Text wildPokemonLevelText;
    [SerializeField] HPBar wildPokemonHPBar;

    [Header("Player Pokemon HUD")]
    [SerializeField] Text playerPokemonNameText;
    [SerializeField] Text playerPokemonLevelText;
    [SerializeField] HPBar playerPokemonHPBar;

    [Header("Moves")]
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> ppText;
    [SerializeField] List<Text> typeText;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    public void SetData(PokemonStatsCalculator wildPokemon, PokemonStatsCalculator playerPokemon)
    {
        wildPokemonNameText.text = wildPokemon.pokemonBase.Name;
        wildPokemonLevelText.text = "Lvl " + wildPokemon.Level;
        wildPokemonHPBar.SetHP((float)wildPokemon.currentHP / wildPokemon.maxHP);
        playerPokemonNameText.text = playerPokemon.pokemonBase.Name;
        playerPokemonLevelText.text = "Lvl " + playerPokemon.Level;
        playerPokemonHPBar.SetHP((float)playerPokemon.currentHP / playerPokemon.maxHP);
        SetMovesUI(playerPokemon.pokemonBase.Move);
    }

    public void SetMovesUI(List<MoveBase> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Name;
            else
                moveTexts[i].text = "-";
        }

        for (int i = 0; i < ppText.Count; ++i)
        {
            if (i < moves.Count)
                ppText[i].text = moves[i].PP.ToString() + "/" + moves[i].PP.ToString();
            else
                ppText[i].text = "-";
        }
        for (int i = 0; i < typeText.Count; ++i)
        {
            if (i < moves.Count)
                typeText[i].text = moves[i].Type.ToString();
            else
                typeText[i].text = "null";
        }
    }

    public IEnumerator UpdateWildPokemonHP()
    {
        yield return wildPokemonHPBar.SetHPSmoothly((float)battleManager.wildPokemonStatsCalculator.currentHP / battleManager.wildPokemonStatsCalculator.maxHP);
    }

    public IEnumerator UpdateMyPokemonHP()
    {
        yield return playerPokemonHPBar.SetHPSmoothly((float)battleManager.pokemonStatsCalculator.currentHP / battleManager.pokemonStatsCalculator.maxHP);
    }
}
