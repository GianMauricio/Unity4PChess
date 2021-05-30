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

    protected override void Awake()
    {
        base.Awake();

        //Get photon view component
        photonView = GetComponent<PhotonView>();
    }

    //This will only be called across multiplayer board,
    //single player boards will mimic this function but only
    //receiving functionality
    protected override void UpdateBoards(Vector2Int originTile, Vector2Int destinationTile)
    {
        photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.AllBuffered, new object[] {originTile, destinationTile});
    }

    [PunRPC]
    private void RPC_OnSelectedPieceMoved(Vector2Int originTile, Vector2Int destinationTile)
    {
        //Get the old tile which the piece WAS on
        Tile oldTile = TileBoard[originTile.x, originTile.y];

        //Move that piece to the tile where it is supposed to be
        Tile newTile = TileBoard[destinationTile.x, destinationTile.y];

        //Invoke movement from piece at origin tile and tell it to move to destination tile. And pray. To all gods
        BasePiece invokedPiece = oldTile.currPiece;
        invokedPiece.InvokedMove(newTile);
    }
}
