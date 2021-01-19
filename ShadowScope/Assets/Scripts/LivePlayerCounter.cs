using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LivePlayerCounter : MonoBehaviour
{
    public GameObject[] activePlayers;
    public TextMeshProUGUI numActivePlayers;
    void Update()
    {
        activePlayers = GameObject.FindGameObjectsWithTag("Player");
        numActivePlayers.text = "Live Players: " + activePlayers.Length;
    }
}
