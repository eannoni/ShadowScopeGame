using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGame : MonoBehaviour
{
    public bool isRedTeam;
    bool isRedWinner;
    public PlayerManager playerManager;
    public AudioClip loseSound;
    public AudioClip victory;
    public AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void SetWinner(int winner)
    {
        if (winner == 0)
            isRedWinner = true;
        else
            isRedWinner = false;
    }

    public void EndGameWinSound()
    {
        source.PlayOneShot(victory);
    }

    public void EndGameLoseSound()
    {
        source.PlayOneShot(loseSound);
    }
}
