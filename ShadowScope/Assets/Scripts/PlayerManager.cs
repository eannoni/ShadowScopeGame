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
        team = -1;
        Debug.Log("In PlayerManager start method");
        if (PV.IsMine) // if PhotonView is owned by the local player
        {
            // assign teams
            Debug.Log("Assigning Teams");
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);

            Debug.Log("Creating Controller");
            CreateController();
        }
        else
        {
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
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
        CreateController();
    }

    // set player's team number (to be sent only to the master client)
    [PunRPC]
    void RPC_GetTeam()
    {
        team = RoomManager.Instance.GetNextTeamNumber();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, team);
    }

    // broadcast players' team numbers to all other clients
    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        team = whichTeam; // set the team number
    }
}
