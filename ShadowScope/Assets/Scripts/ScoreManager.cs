﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public Menu scoreMenu;
    public Menu winnerMenu;

    public TMP_Text redKillsText;
    public TMP_Text blueKillsText;

    public TMP_Text redWinText;
    public TMP_Text blueWinText;

    public int redKills;
    public int blueKills;

    public int killsNeeded;

    void Awake()
    {
        killsNeeded = (5 * PhotonNetwork.CurrentRoom.PlayerCount) / 2; ;

        // standard Singleton pattern
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        MenuManager.Instance.OpenMenu(scoreMenu);

        redKills = 0;
        blueKills = 0;
        SetScoreText();
        Debug.Log("4");
    }

    public int IsWinner()
    {
        if (redKills >= killsNeeded)
            return 0;
        else if (blueKills >= killsNeeded)
            return 1;
        return -1;
    }

    public void SetScoreText()
    {
        redKillsText.text = "Red Kills: " + redKills + " / " + killsNeeded;
        blueKillsText.text = "Blue Kills: " + blueKills + " / " + killsNeeded;
    }

    public void DisplayWinner()
    {
        MenuManager.Instance.OpenMenu(winnerMenu);

        if (redKills >= killsNeeded)
            redWinText.gameObject.SetActive(true);
        else
            blueWinText.gameObject.SetActive(true);
    }
}