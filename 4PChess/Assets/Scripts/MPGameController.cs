using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MPGameController : GameController
{
    private Color localColor;
    public NetworkSocket networkManager;
    

    //On Enable
    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void setLocalPlayerColor(Color color)
    {
        localColor = color;
    }

    protected override void SetGameState(GameState newState)
    {
        
    }

    public override void TryStartGame()
    {
        if (networkManager.IsRoomFull())
        {
            SetGameState(GameState.inPlay);
        }
    }

    
}
