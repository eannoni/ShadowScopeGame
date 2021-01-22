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

    [SerializeField] TMP_InputField nicknameInputField;
    [SerializeField] TMP_Text nameSetText;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [SerializeField] Transform playerListContentRed;
    [SerializeField] Transform playerListContentBlue;
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

    public void SetNickname()
    {
        if (!string.IsNullOrEmpty(nicknameInputField.text))
        {
            PhotonNetwork.NickName = nicknameInputField.text;
            StartCoroutine(SetNicknameText());
        }
    }

    // displays nickname successful text briefly
    IEnumerator SetNicknameText()
    {
        nameSetText.text = "Name set to \"" + PhotonNetwork.NickName + "\".";
        nameSetText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        nameSetText.gameObject.SetActive(false);
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
        if (string.IsNullOrEmpty(roomNameInputField.text))
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

        // force joining red team
        JoinTeam(0);

        RefreshRoomDisplay();
    }

    void RefreshRoomDisplay()
    {
        // clears all players in playerListContentRed
        foreach (Transform child in playerListContentRed)
            Destroy(child.gameObject);

        // clears all players in playerListContentBlue
        foreach (Transform child in playerListContentBlue)
            Destroy(child.gameObject);

        Player[] players = PhotonNetwork.PlayerList;

        // adds players into respective team content fields
        for (int i = 0; i < players.Count(); ++i)
        {
            // if player has a Team custom property
            if (players[i].CustomProperties.ContainsKey("Team"))
            {
                // if player is on red team, instantiate red PlayerListItemPrefab
                if (players[i].CustomProperties["Team"].Equals(0))
                    Instantiate(PlayerListItemPrefab, playerListContentRed).GetComponent<PlayerListItem>().SetUp(players[i]);
                // if player is on blue team, instantiate blue PlayerListItemPrefab
                else if (players[i].CustomProperties["Team"].Equals(1))
                    Instantiate(PlayerListItemPrefab, playerListContentBlue).GetComponent<PlayerListItem>().SetUp(players[i]);
                else
                    Debug.Log("----Uh oh, player could not be put in a group");
            }
        }

        // only lets the host be able to start the game
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // clicked by "Join Red/Blue Team" buttons
    public void JoinTeam(int team)
    {
        // do we already have a team?
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            // remove team property in order to trigger OnPlayerPropertiesUpdate
            PhotonNetwork.RemovePlayerCustomProperties(new string[]{"Team"});
        }

        // set the player properties of this client to the team they clicked
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps.Add("Team", team);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        RefreshRoomDisplay();
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
        // this applies to all players in game, rather than unity loading the scene which would only trigger the host's scene
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
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        // instantiate roomListItemPrefabs inside RoomListItem container
        for (int i = 0; i < roomList.Count; ++i)
        {
            // if roomListItem has been removed, don't instantiate
            if (roomList[i].RemovedFromList)
                continue;

            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContentRed).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // find player that joined and update their properties (may not be necessary)
        for (int i = 0; i < PhotonNetwork.PlayerList.Count(); ++i)
        {
            if (PhotonNetwork.PlayerList[i] == targetPlayer)
                PhotonNetwork.PlayerList[i].CustomProperties = targetPlayer.CustomProperties;
        }
        RefreshRoomDisplay();
    }
}
