﻿using System.Collections;
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

    public void EndGameSounds()
    {
        if (isRedTeam)
        {
            if (isRedWinner)
            {
                Debug.Log("Red team win");
                source.PlayOneShot(victory);
            }
            else
            {
                Debug.Log("Red team loss");
                source.PlayOneShot(loseSound);
            }
        }
        else
        {
            if (!isRedWinner)
            {
                Debug.Log("Blue team win");
                source.PlayOneShot(victory);
            }
            else
            {
                Debug.Log("Blue team loss");
                source.PlayOneShot(loseSound);
            }
        }
    }
}
