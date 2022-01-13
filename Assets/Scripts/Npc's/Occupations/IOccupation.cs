using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOccupation
{
    string Name { get; }
    void Trigger(GameObject other);
}
