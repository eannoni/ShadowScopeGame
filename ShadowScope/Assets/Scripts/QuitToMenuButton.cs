using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class QuitToMenuButton : MonoBehaviourPunCallbacks
{
    public void OnQuitClick()
    {
        Debug.Log("In disconnect and load coroutine");
        Destroy(RoomManager.Instance.gameObject);
        Destroy(ScoreManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("On left room");
        SceneManager.LoadScene(0);
        base.OnLeftRoom();
        MenuManager.Instance.OpenMenu("title");
    }
}
