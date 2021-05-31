using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkSocket : MonoBehaviourPunCallbacks
{
    public UIManagement uiManager;
    private MPGameController chessGameController;

    private const string LEVEL = "level";
    private const string TEAM = "team";
    private const byte MAX_PLAYERS = 4;

    private Board chessBoard;

    void Awake()
    {
        //Do not delete this or the update calls will die
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //Set dependencies
    public void SetDependencies(MPGameController gameController)
    {
        this.chessGameController = gameController;
    }

    void Update()
    {
        //Update connection UI
        uiManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinRandomRoom();
        }

        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //Custom implementation for network callback functions
    //On connected to master server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        PhotonNetwork.JoinRandomRoom();
    }

    public bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public int getPlayersInRoom()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    //On joined room failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Found no rooms due to {message} making a new room instead.");
        PhotonNetwork.CreateRoom(null); //Because we want all rooms to be "joinable"
    }

    //On joined room success
    public override void OnJoinedRoom()
    {
        Debug.Log($"Local player {PhotonNetwork.LocalPlayer.ActorNumber} has joined");

        //Tell player to choose team
        PrepareTeamSelectionOptions();
        uiManager.showTeamSelect();
    }

    //Callback if another player has entered the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Remote player {newPlayer.ActorNumber} has joined");

        if (newPlayer.ActorNumber == 4)
        {
            Debug.Log($"Last Player has joined, game is ready");
        }
    }

    //Set own team
    public void setTeam(int team)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable{ {TEAM, team} });

        //If this player is the last to select a team, start the game
        if (PhotonNetwork.LocalPlayer.ActorNumber == MAX_PLAYERS)
        {
            uiManager.tryStartGame();
            Debug.Log("Trying to start game");
        }
    }

    //Private usage functions
    //Function to make certain teams unavailable
    private void PrepareTeamSelectionOptions()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (i == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                continue;
            }

            var player = PhotonNetwork.CurrentRoom.GetPlayer(i);
            if (player.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = player.CustomProperties[TEAM];
                uiManager.RestrictTeamChoice((int)occupiedTeam);
            }
        }
    }
}
