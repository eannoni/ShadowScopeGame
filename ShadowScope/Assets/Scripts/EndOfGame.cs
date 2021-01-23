using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGame : MonoBehaviour
{
    bool isRedTeam;
    bool isRedWinner;
    public PlayerManager playerManager;
    public AudioClip loseSound;
    public AudioClip victory;
    public AudioSource source;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (playerManager.team == 0)
            isRedTeam = true;
        else
            isRedTeam = false;
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
