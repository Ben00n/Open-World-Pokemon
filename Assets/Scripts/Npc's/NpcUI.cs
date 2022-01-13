using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI npcNameText = null;

    [SerializeField] private TextMeshProUGUI npcGreetingText = null;

    [SerializeField] private Transform occupationButtonHolder = null;

    [SerializeField] private GameObject occupationButtonPrefab = null;

    [SerializeField] int lettersPerSecond;

    public IEnumerator TypeDialog(string line)
    {
        npcGreetingText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            npcGreetingText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void SetNpc(Npc npc)
    {
        npcNameText.text = npc.Name + ":";
        npcGreetingText.text = npc.GreetingText;
        StartCoroutine(TypeDialog(npcGreetingText.text));

        foreach(Transform child in occupationButtonHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < npc.Occupations.Length; i++)
        {
            GameObject buttonInstance = Instantiate(occupationButtonPrefab, occupationButtonHolder);

            buttonInstance.GetComponent<OccupationButton>().Initialise(npc.Occupations[i], npc.OtherInteractor);
        }
    }
}
