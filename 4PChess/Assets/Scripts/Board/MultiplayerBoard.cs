using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//Require the view component or death
[RequireComponent(typeof(PhotonView))]
public class MultiplayerBoard : Board
{
    //THIS MUST ONLY EXIST ONCE OR DEATH
    private PhotonView photonView;
    public GameObject turnUI;

    protected override void Awake()
    {
        base.Awake();

        //Get photon view component
        photonView = GetComponent<PhotonView>();

        //This is bad implementation... Too bad!
        turnUI = GameObject.Find("TurnUI");
    }

    //This will only be called across multiplayer board,
    //single player boards will mimic this function but only
    //receiving functionality
    public override void UpdateBoards(Vector2 originTile, Vector2 destinationTile)
    {
        photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.AllBuffered, new object[] {originTile, destinationTile});
    }

    public override void nextTurn(int Player)
    {
        photonView.RPC(nameof(RPC_NextTurn), RpcTarget.AllBuffered, new object[] { Player });

        Color color = Color.clear;
        if (Player == 1)
        {
            color = Color.white;
        }

        else if (Player == 2)
        {
            color = Color.red;
        }

        else if (Player == 3)
        {
            color = Color.black;
        }

        else if (Player == 4)
        {
            color = Color.blue;
        }

        //Change own UI colors
        if (color == Color.blue)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.white;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 1";
        }
        else if (color == Color.white)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 2";
        }
        else if (color == Color.red)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.black;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 3";
        }
        else if (color == Color.black)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.blue;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 4";
        }
    }

    [PunRPC]
    private void RPC_OnSelectedPieceMoved(Vector2 originTile, Vector2 destinationTile)
    {
        Vector2Int OriginCoords = new Vector2Int(Mathf.RoundToInt(originTile.x), Mathf.RoundToInt(originTile.y));
        Vector2Int DestCoords = new Vector2Int(Mathf.RoundToInt(destinationTile.x), Mathf.RoundToInt(destinationTile.y));

        //Debug.Log("Transferring piece from " + OriginCoords + " to " + DestCoords);

        //Get the old tile which the piece WAS on
        Tile oldTile = TileBoard[OriginCoords.x, OriginCoords.y];

        //Move that piece to the tile where it is supposed to be
        Tile newTile = TileBoard[DestCoords.x, DestCoords.y];

        Debug.Log("Transferring piece from " + oldTile.BoardPos + " to " + newTile.BoardPos);

        //Invoke movement from piece at origin tile and tell it to move to destination tile. And pray. To all gods
        BasePiece invokedPiece = oldTile.currPiece;
        invokedPiece.InvokedMove(newTile);
    }

    [PunRPC]
    private void RPC_NextTurn(int Player)
    {
        base.nextTurn(Player);
    }
}
