using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public GameObject ActionSelector;
    public GameObject battleDialogBox;
    BattleManager battleManager;

    [Header("Wild Pokemon HUD")]
    [SerializeField] Text wildPokemonNameText;
    [SerializeField] Text wildPokemonLevelText;
    [SerializeField] Text wildPokemonStatusText;
    [SerializeField] public HPBar wildPokemonHPBar;

    [Header("Player Pokemon HUD")]
    [SerializeField] Text playerPokemonNameText;
    [SerializeField] Text playerPokemonLevelText;
    [SerializeField] Text playerPokemonStatusText;
    [SerializeField] public HPBar playerPokemonHPBar;

    [Header("Moves")]
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> ppText;
    [SerializeField] List<Text> typeText;

    [Header("Status Colors")]
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Dictionary<ConditionID, Color> statusColors;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    private void SetStatusText()
    {
        if (battleManager.playerPokemonStatsCalculator.Status == null)
        {
            playerPokemonStatusText.text = "";
        }
        else
        {
            playerPokemonStatusText.text = battleManager.playerPokemonStatsCalculator.Status.Id.ToString().ToUpper();
            playerPokemonStatusText.color = statusColors[battleManager.playerPokemonStatsCalculator.Status.Id];
        }
        if (battleManager.wildPokemonStatsCalculator.Status == null)
        {
            wildPokemonStatusText.text = "";
        }
        else
        {
            wildPokemonStatusText.text = battleManager.wildPokemonStatsCalculator.Status.Id.ToString().ToUpper();
            wildPokemonStatusText.color = statusColors[battleManager.wildPokemonStatsCalculator.Status.Id];
        }
    }

    public void SetData(PokemonStatsCalculator wildPokemon, PokemonStatsCalculator playerPokemon)
    {
        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.psn, psnColor },
            { ConditionID.brn, brnColor },
            { ConditionID.slp, slpColor },
            { ConditionID.par, parColor },
            { ConditionID.frz, frzColor },
        };

        wildPokemonNameText.text = wildPokemon.pokemonBase.Name;
        wildPokemonLevelText.text = "Lvl " + wildPokemon.Level;
        wildPokemonHPBar.SetHP((float)wildPokemon.currentHP / wildPokemon.maxHP);
        playerPokemonNameText.text = playerPokemon.pokemonBase.Name;
        SetLevel(); // set player pokemon lvl similar to 2 lines up for wild
        playerPokemonHPBar.SetHP((float)playerPokemon.currentHP / playerPokemon.maxHP);
        SetMovesUI(playerPokemon.Moves);
        SetStatusText();
        battleManager.playerPokemonStatsCalculator.OnStatusChanged += SetStatusText;
        battleManager.wildPokemonStatsCalculator.OnStatusChanged += SetStatusText;
    }

    public void SetMovesUI(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }

        for (int i = 0; i < ppText.Count; ++i)
        {
            if (i < moves.Count)
                ppText[i].text = moves[i].PP.ToString() + "/" + moves[i].Base.maximumPP.ToString();
            else
                ppText[i].text = "-";
        }
        for (int i = 0; i < typeText.Count; ++i)
        {
            if (i < moves.Count)
                typeText[i].text = moves[i].Base.Type.ToString();
            else
                typeText[i].text = "null";
        }
    }

    public void SetLevel()
    {
        playerPokemonLevelText.text = "Lvl " + battleManager.playerPokemonStatsCalculator.Level;
    }

    //public void SetExp()
    //{
    //    if (expBar == null) return;

    //    float normalizedExp = GetNormalizedExp();
    //    expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    //}
    //public IEnumerator SetExpSmooth(bool reset=false)
    //{
    //    if (expBar == null) yield break;

    //    if(reset)
    //        expBar.transform.localScale = new Vector3(0, 1, 1);

    //    float normalizedExp = GetNormalizedExp();
    //    yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    //}

    //float GetNormalizedExp()
    //{
    //    int currLevelExp = battleManager.playerPokemonStatsCalculator.pokemonBase.GetExpForLevel(battleManager.playerPokemonStatsCalculator.Level);
    //    int nextLevelExp = battleManager.playerPokemonStatsCalculator.pokemonBase.GetExpForLevel(battleManager.playerPokemonStatsCalculator.Level + 1);

    //    float normalizedExp = (float) (battleManager.playerPokemonStatsCalculator.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
    //    return Mathf.Clamp01(normalizedExp);
    //}

    public IEnumerator UpdatePokemonHP(PokemonStatsCalculator pokemon,HPBar hpBar)
    {
        yield return hpBar.SetHPSmoothly((float)pokemon.currentHP / pokemon.maxHP);
    }
}
