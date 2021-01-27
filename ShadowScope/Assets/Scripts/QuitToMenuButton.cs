using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuitToMenuButton : MonoBehaviour
{
    public void OnQuitClick()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(QuitGame());
    }

    IEnumerator QuitGame()
    {
        yield return new WaitWhile(() => PhotonNetwork.IsConnected);
        Application.Quit();
    }
}
