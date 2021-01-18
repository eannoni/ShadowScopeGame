using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //Photon Unity Networking
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks //gives access to callbacks for room creation, errors, joining lobbies, etc.
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        // connect to Photon master server using our settings in PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
    }

    // called by Photon when we successfully connect to the master server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        // lobbies are where you find/create rooms
        PhotonNetwork.JoinLobby();
        // automatically loads scenes for all other clients when host changes scenes
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("title");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000"); // temporary username system
    }

    public void CreateRoom()
    {
        // make sure user doesn't generate room with empty name
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    // successfully created/joined a room
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        // clears all players in playerListContent
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); ++i)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        // only lets the host be able to start the game
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // called if there is a host migration, which is if host leaves, another player becomes host
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // give new host access to start game button
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // failed to create room
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        // 1 is the scene index in the Build Settings (Game scene)
        // using PhotonNetwork applies to all players in game, rather than using Unity to load the scene, which would only trigger the host's scene
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    // roomList gives room information (name, max players, properties, etc)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // clear the list each update
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        // instantiate roomListItemPrefabs inside RoomListItem container
        for (int i = 0; i < roomList.Count; ++i)
        {
            // if roomListItem has been removed, don't instantiate
            if(roomList[i].RemovedFromList)
                continue;

            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
