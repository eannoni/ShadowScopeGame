using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private int nextTeamNumber; // oscillates between 0 and 1 for assigning team values

    void Awake()
    {
        // standard Singleton pattern
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        nextTeamNumber = -1;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // in the game scene
        {
            // instantiates prefab from PhotonPrefab folder in Resources?
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    // NOT CURRENTLY WORKING
    IEnumerator DisconnectAndLoad()
    {
        Debug.Log("Disconnecting...");
        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        //while (PhotonNetwork.IsConnected)
        while(PhotonNetwork.InRoom)
            yield return null;

        Debug.Log("Disconnected");
        OnDisable();
        SceneManager.LoadScene(0);
        Debug.Log("Scene Loaded");
    }

    // returns next team number (0 or 1) and increments
    public int GetNextTeamNumber()
    {
        return ++nextTeamNumber % 2;
    }
}
