using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public const int killsNeeded = 15;

    void Awake()
    {
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
    }

    public bool IsWinner()
    {
        if (redKills >= killsNeeded || blueKills >= killsNeeded)
            return true;
        return false;
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