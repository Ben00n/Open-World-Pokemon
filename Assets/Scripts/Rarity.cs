using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Rarity")]
public class Rarity : ScriptableObject
{
    [SerializeField] private new string name = "New Rarity Name";
    [SerializeField] private Color textColour = new Color(1f, 1f, 1f, 1f);

    public string Name => name;
    public Color TextColour => textColour;
}
