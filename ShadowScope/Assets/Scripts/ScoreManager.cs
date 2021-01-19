using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public Menu scoreMenu;
    public Menu winnerMenu;

    public TMP_Text redLivesText;
    public TMP_Text blueLivesText;

    public TMP_Text redWinText;
    public TMP_Text blueWinText;

    public int redLives;
    public int blueLives;

    public const int maxLives = 3;

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

        redLives = maxLives;
        blueLives = maxLives;
        SetScoreText();
    }

    public bool IsWinner()
    {
        if (redLives <= 0 || blueLives <= 0)
            return true;
        return false;
    }

    public void SetScoreText()
    {
        redLivesText.text = "Red Lives: " + redLives;
        blueLivesText.text = "Blue Lives: " + blueLives;
    }

    public void DisplayWinner()
    {
        MenuManager.Instance.OpenMenu(winnerMenu);

        if (redLives <= 0)
            blueWinText.gameObject.SetActive(true);
        else
            redWinText.gameObject.SetActive(true);
    }
}