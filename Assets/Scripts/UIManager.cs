using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject battleHUD;
    PlayerManager playerManager;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        CheckIfIsInBattleAndEnableUIForBattle();
    }

    private void CheckIfIsInBattleAndEnableUIForBattle()
    {
        if(playerManager.isInBattle)
        {
            battleHUD.SetActive(true);
        }
        else
        {
            battleHUD.SetActive(false);
        }
    }
}
