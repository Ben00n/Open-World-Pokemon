using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (PokemonStatsCalculator pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.maxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is hurt due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (PokemonStatsCalculator pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.maxHP/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is hurt due to burn");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                OnBeforeMove = (PokemonStatsCalculator pokemon) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is paralyzed and can't move!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                OnBeforeMove = (PokemonStatsCalculator pokemon) =>
                {
                    if (Random.Range(1,5) == 1) // between 1 and 4
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is not frozen anymore!");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is frozen and unable to attack!");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStart = (PokemonStatsCalculator pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },

                OnBeforeMove = (PokemonStatsCalculator pokemon) =>
                {
                    if(pokemon.StatusTime == 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is asleep and unable to attack!");
                    return false;
                }
            }
        },
        //Volatile Status Conditions
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused",
                OnStart = (PokemonStatsCalculator pokemon) =>
                {
                    //Confused for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");
                },

                OnBeforeMove = (PokemonStatsCalculator pokemon) =>
                {
                    if(pokemon.VolatileStatusTime == 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} kicked out of confusion!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;

                    //50% chance to do a move
                    if (Random.Range(1,3) == 1)
                        return true;

                    //Hurt by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} is confused!");
                    pokemon.UpdateHP(pokemon.maxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} hurt itself due to confusion");
                    return false;
                }
            }
        },

        //Weather Conditions
        {
            ConditionID.sunny,
            new Condition()
            {
                Name = "Harsh Sunlight",
                StartMessage = "The weather has changed to Harsh Sunlight",
                EffectMessage = "The sunlight is harsh",
                OnDamageModify = (PokemonStatsCalculator source, PokemonStatsCalculator target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Fire)
                        return 1.5f;
                    else if (move.Base.Type == PokemonType.Water)
                        return 0.5f;

                    return 1f;
                }
            }
        },

        {
            ConditionID.rain,
            new Condition()
            {
                Name = "Heavy Rain",
                StartMessage = "It started raining heavily",
                EffectMessage = "It's raining heavily",
                OnDamageModify = (PokemonStatsCalculator source, PokemonStatsCalculator target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Water)
                        return 1.5f;
                    else if (move.Base.Type == PokemonType.Fire)
                        return 0.5f;

                    return 1f;
                }
            }
        },

        {
            ConditionID.sandstorm,
            new Condition()
            {
                Name = "Sandstorm",
                StartMessage = "A sandstorm starts to rage",
                EffectMessage = "The sandstorm is raging!",
                OnWeather = (PokemonStatsCalculator pokemon) =>
                {
                    pokemon.UpdateHP(Mathf.RoundToInt((float)pokemon.maxHP / 16f));
                    pokemon.StatusChanges.Enqueue($"{pokemon.pokemonBase.Name} has been buffeted by the sandstorm!");
                }
            }
        },




    };
}

public enum ConditionID
{
    none,psn,brn,slp,par,frz,
    confusion,
    sunny, rain, sandstorm
}
