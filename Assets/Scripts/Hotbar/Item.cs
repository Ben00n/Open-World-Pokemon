using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private new string name = "New Item Name";
    [SerializeField] Sprite icon = null;

    public string Name => name;
    public abstract string ColouredName { get; }
    public Sprite Icon => icon;
    public abstract string GetInfoDisplayText();

}
