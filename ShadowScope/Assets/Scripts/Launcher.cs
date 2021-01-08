using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //Photon Unity Networking
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks //gives access to callbacks for room creation, errors, joining lobbies, etc.
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

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
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("title");
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
    }

    // failed to create room
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
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
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
}
