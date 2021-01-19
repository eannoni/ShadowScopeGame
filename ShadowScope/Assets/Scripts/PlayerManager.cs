using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    public int team; // 0: red, 1: blue

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Debug.Log("In PlayerManager Start");

        // assign teams
        team = -1;
        PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);

        if (PV.IsMine) // if PhotonView is owned by the local player
        {
            Debug.Log("Creating Controller");
            CreateController();
        }
    }

    // Instantiates our player controller
    void CreateController()
    {
        // get spawnpoint from SpawnManager
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint(team);
        // instantiate controller
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    // destroy controller and re-instantiate it
    public void Die()
    {
        PhotonNetwork.Destroy(controller);

        // set score
        if(team == 0)
            ScoreManager.Instance.redLives--;
        else
            ScoreManager.Instance.blueLives--;

        bool isWinner = ScoreManager.Instance.IsWinner();

        // update score on all clients
        PV.RPC("RPC_SetScoreText", RpcTarget.All, ScoreManager.Instance.redLives, ScoreManager.Instance.blueLives, isWinner);

        CreateController();
    }

    // set player's team number (to be sent only to the master client)
    [PunRPC]
    void RPC_GetTeam()
    {
        Debug.Log("RPC GetTeam");
        team = RoomManager.Instance.GetNextTeamNumber();
        PV.RPC("RPC_SentTeam", RpcTarget.Others, team);
    }

    // broadcast players' team numbers to all other clients
    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        Debug.Log("RPC SetTeam");
        team = whichTeam; // set the team number
        Debug.Log("inside RPC SetTeam...team = " + team);
    }

    // sets new scores and if winner, queues winner menu
    [PunRPC]
    void RPC_SetScoreText(int newRedLives, int newBlueLives, bool isWinner)
    {
        // update scores
        ScoreManager.Instance.redLives = newRedLives;
        ScoreManager.Instance.blueLives = newBlueLives;

        // display scores
        ScoreManager.Instance.SetScoreText();

        if (isWinner)
            ScoreManager.Instance.DisplayWinner();
    }
}
