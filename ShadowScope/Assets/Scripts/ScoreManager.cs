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

    public int killsNeeded;

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
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");
        killsNeeded = (5 * activePlayers.Length) / 2;

        redKills = 0;
        blueKills = 0;
        SetScoreText();
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