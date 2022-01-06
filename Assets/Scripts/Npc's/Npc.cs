using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private new string name = "New Npc Name";
    [SerializeField] private string greetingText = "Hello adventurer";
    private IOccupation[] occupations = new IOccupation[0];

    private void Start()
    {
        occupations = GetComponents<IOccupation>();
    }

    public void Interact(GameObject other)
    {
        Debug.Log($"{name}: {greetingText}.");

        for (int i = 0; i < occupations.Length; i++)
        {
            Debug.Log(occupations[i].Name);
            Debug.Log(occupations[i].Data);

        }
    }
}
