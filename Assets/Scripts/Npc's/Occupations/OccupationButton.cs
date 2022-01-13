using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OccupationButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI occupationNameText = null;

    private IOccupation occupation = null;
    private GameObject other = null;

    public void Initialise(IOccupation occupation, GameObject other)
    {
        this.occupation = occupation;
        this.other = other;
    }

    public void TriggerOccupation() => occupation.Trigger(other);
}
