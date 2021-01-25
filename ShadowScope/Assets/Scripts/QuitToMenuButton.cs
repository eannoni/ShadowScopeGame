using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class QuitToMenuButton : MonoBehaviour
{
    public void OnQuitClick()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
