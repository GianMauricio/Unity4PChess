using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MPGameController : GameController, IOnEventCallback
{
    //On Enable
    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected override void SetGameState(GameState newState)
    {
        throw new System.NotImplementedException();
    }

    public override void TryStartGame()
    {
        throw new System.NotImplementedException();
    }

    public void OnEvent(EventData photonEvent)
    {
        throw new System.NotImplementedException();
    }
}
