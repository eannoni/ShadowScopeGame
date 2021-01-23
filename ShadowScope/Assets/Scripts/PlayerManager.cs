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
    }

    void Start()
    {
        // assign team
        team = (int)PV.Owner.CustomProperties["Team"];

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

        bool isWinner;
        if (ScoreManager.Instance.IsWinner() > -1)
            isWinner = true;
        else
            isWinner = false;



        // update score on all clients
        PV.RPC("RPC_SetScoreText", RpcTarget.All, ScoreManager.Instance.redKills, ScoreManager.Instance.blueKills, isWinner);

        CreateController();
    }

    // sets new scores and if winner, queues winner menu
    [PunRPC]
    void RPC_SetScoreText(int redKills, int blueKills, bool isWinner)
    {
        // update scores
        ScoreManager.Instance.redKills = redKills;
        ScoreManager.Instance.blueKills = blueKills;

        // display scores
        ScoreManager.Instance.SetScoreText();

        if (isWinner)
        {
            ScoreManager.Instance.DisplayWinner();
            end.SetWinner(ScoreManager.Instance.IsWinner());
            end.EndGameSounds();
        }
    }
}
