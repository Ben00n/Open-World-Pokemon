using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public string EffectMessage { get; set; }
    public Action<PokemonStatsCalculator> OnStart { get; set; }
    public Func<PokemonStatsCalculator, bool> OnBeforeMove { get; set; }
    public Action<PokemonStatsCalculator> OnAfterTurn { get; set; }
    public Action<PokemonStatsCalculator> OnWeather { get; set; }
    public Func<PokemonStatsCalculator, PokemonStatsCalculator, Move, float> OnDamageModify { get; set; }



}
