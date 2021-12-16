using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBar : MonoBehaviour
{
    BattleManager battleManager;

    [SerializeField] GameObject expBar;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = battleManager.playerPokemonStatsCalculator.pokemonBase.GetExpForLevel(battleManager.playerPokemonStatsCalculator.Level);
        int nextLevelExp = battleManager.playerPokemonStatsCalculator.pokemonBase.GetExpForLevel(battleManager.playerPokemonStatsCalculator.Level + 1);

        float normalizedExp = (float)(battleManager.playerPokemonStatsCalculator.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

}
