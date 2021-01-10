using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine) // if PhotonView is owned by the local player
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // Instantiate our player controller
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
    }
}
