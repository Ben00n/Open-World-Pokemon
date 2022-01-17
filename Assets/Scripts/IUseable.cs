using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseable
{
    ItemType itemType { get; }
    void Use();
}

public enum ItemType
{
    Pokeball,
    Revive,
}