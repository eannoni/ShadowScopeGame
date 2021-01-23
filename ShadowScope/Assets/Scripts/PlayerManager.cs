using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    //public AudioClip victorySound;
    //public AudioClip loseSound;
    //public AudioSource audio;
    EndOfGame end;

    public int team; // 0: red, 1: blue

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        //audio = GetComponent<AudioSource>();
        end = GetComponentInChildren<EndOfGame>();
    }

    void Start()
    {
        // assign team
        team = (int)PV.Owner.CustomProperties["Team"];
        if (team == 0)
            end.isRedTeam = true;
        else
            end.isRedTeam = false;

        if (PV.IsMine) // if PhotonView is owned by the local player
            CreateController();
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
            ScoreManager.Instance.blueKills++;
        else
            ScoreManager.Instance.redKills++;

        int winner = ScoreManager.Instance.IsWinner();

        // update score on all clients
        PV.RPC("RPC_SetScoreText", RpcTarget.All, ScoreManager.Instance.redKills, ScoreManager.Instance.blueKills);

        // display winner and play sounds
        if(winner > -1)
            PV.RPC("RPC_EndGame", RpcTarget.All, winner);

        CreateController();
    }

    // sets new scores and if winner, queues winner menu
    [PunRPC]
    void RPC_SetScoreText(int redKills, int blueKills)
    {
        // update scores
        ScoreManager.Instance.redKills = redKills;
        ScoreManager.Instance.blueKills = blueKills;

        // display scores
        ScoreManager.Instance.SetScoreText();
    }

    [PunRPC]
    void RPC_EndGame(int winner)
    {
        ScoreManager.Instance.DisplayWinner();

        int localTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

        if (localTeam == winner)
            end.EndGameWinSound();
        else
            end.EndGameLoseSound();
    }
}
