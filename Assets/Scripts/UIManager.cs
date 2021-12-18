﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject battleHUD;
    public GameObject partyList;
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
        if (playerManager.isInBattle)
        {
            partyList.SetActive(false);
            battleHUD.SetActive(true);
        }
        else
        {
            partyList.SetActive(true);
            battleHUD.SetActive(false);
        }
    }
}
