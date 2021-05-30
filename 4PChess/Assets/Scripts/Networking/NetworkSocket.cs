using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkSocket : MonoBehaviourPunCallbacks
{
    public UIManagement uiManager;

    private const string LEVEL = "level";
    private const string TEAM = "team";
    private const byte MAX_PLAYERS = 4;

    private Board chessBoard;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
            PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, chessBoard } }, MAX_PLAYERS);
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
        Debug.LogAssertion("Connected to server");
        PhotonNetwork.JoinRandomRoom();
    }

    //On joined room failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Found no rooms due to {message} making a new room instead.");
        PhotonNetwork.CreateRoom(null); //Because we want all rooms to be "joinable"
    }

    //On joined room success
    public override void OnJoinedRoom()
    {
        Debug.LogAssertion($"Local player {PhotonNetwork.LocalPlayer.ActorNumber} has joined");
    }

    //Callback if another player has entered the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogAssertion($"Remote player {newPlayer.ActorNumber} has joined");
    }
}
