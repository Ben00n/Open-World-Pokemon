using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public Condition Weather { get; set; }

    public int? WeatherDuration { get; set; }

    public void SetWeather(ConditionID conditionId)
    {
        Weather = ConditionsDB.Conditions[conditionId];
        Weather.Id = conditionId;
        Weather.OnStart?.Invoke(null);
    }
}
