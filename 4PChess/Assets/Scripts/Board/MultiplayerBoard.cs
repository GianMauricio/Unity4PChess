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
        photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.All, new object[] {originTile, destinationTile});
    }

    public override void nextTurn(int Player)
    {
        photonView.RPC(nameof(RPC_NextTurn), RpcTarget.AllBuffered, new object[] { Player });
    }

    public override void killPlayer(int targetPlayer)
    {
        base.killPlayer(targetPlayer);

        if(allKingsAlive <= 1)
            photonView.RPC(nameof(RPC_KillPlayer), RpcTarget.AllBuffered, new object[] { targetPlayer });
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

        //Check if player is alive, then give turn to them
        //If player is not alive then check the next guy
        Color color = Color.clear;
        while (color == Color.clear)
        {
            if (Player == 1 && P1Alive)
            {
                color = Color.white;
            }

            else if (Player == 2 && P2Alive)
            {
                color = Color.red;
            }

            else if (Player == 3 && P3Alive)
            {
                color = Color.black;
            }

            else if (Player == 4 && P4Alive)
            {
                color = Color.blue;
            }

            //If all checks fail, increment and loop player variable until someone comes up alive
            if (Player == 4)
            {
                Player = 0;
            }

            else Player++;
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

        Color nextColor = Color.clear;
        //Set interactivity to default pieces
        if (color == Color.blue)
        {
            nextColor = Color.white;
            setInteractive(p1Pieces, true);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }

        else if (color == Color.white)
        {
            nextColor = Color.red;
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, true);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }

        else if (color == Color.red)
        {
            nextColor = Color.black;
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, true);
            setInteractive(p4Pieces, false);
        }

        else if (color == Color.black)
        {
            nextColor = Color.blue;
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, true);
        }

        //Set interactivity of promoted pieces
        foreach (BasePiece piece in promotedPieces)
        {
            piece.enabled = piece.defColor == nextColor;
        }
    }

    [PunRPC]
    private void RPC_KillPlayer(int targetPlayer)
    {
        if (targetPlayer == 1)
        {
            P1Alive = false;
        }

        else if (targetPlayer == 2)
        {
            P2Alive = false;
        }

        else if (targetPlayer == 3)
        {
            P3Alive = false;
        }

        else if (targetPlayer == 4)
        {
            P4Alive = false;
        }

        //Check if all but one player is dead
        if (allKingsAlive == 1)
        {
            if(P1Alive)
                UIManager.setWinnerAndEnd(1);
            else if(P2Alive)
                UIManager.setWinnerAndEnd(2);
            else if (P3Alive)
                UIManager.setWinnerAndEnd(3);
            else if (P4Alive)
                UIManager.setWinnerAndEnd(4);
        }
    }
}
