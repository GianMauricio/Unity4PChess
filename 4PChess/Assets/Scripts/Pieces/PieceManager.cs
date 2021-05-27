﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public GameObject PieceTemplate;
    public GameObject turnUI;

    [HideInInspector] 
    public int allKingsAlive = 4; //Track whether someone has died yet

    //Piece arrays
    private List<BasePiece> p1Pieces = null; //Up --> Down
    private List<BasePiece> p2Pieces = null; //Down --> Up
    private List<BasePiece> p3Pieces = null; //Left --> Right
    private List<BasePiece> p4Pieces = null; //Right --> Left

    private List<BasePiece> promotedPieces = new List<BasePiece>();

    //Order of placement
    private string[] placementOrderHorizontal =
    {
        "vP", "vP", "vP", "vP", "vP", "vP", "vP", "vP",
        "R", "N", "B", "K", "Q", "B", "N", "R"
    };

    //Order of placement
    private string[] placementOrderVertical =
    {
        "hP", "hP", "hP", "hP", "hP", "hP", "hP", "hP",
        "R", "N", "B", "K", "Q", "B", "N", "R"
    };

    //Keep stuff in order for easier spawning and logic query
    private Dictionary<string, Type> pieceLibrary = new Dictionary<string, Type>()
    {
        {"hP", typeof(PawnPiece)},
        {"vP", typeof(vPawnPiece)},
        {"B", typeof(BishopPiece)},
        {"N", typeof(KnightPiece)},
        {"R", typeof(RookPiece)},
        {"Q", typeof(QueenPiece)},
        {"K", typeof(KingPiece)}
    };

    //Set pieces to board
    public void Setup(Board boardMain)
    {
        //Make the "white" pieces
        p1Pieces = CreatePieceListHorizontal(Color.white, new Color32(255, 255, 179, 255), boardMain);

        //Make the "red" pieces
        p2Pieces = CreatePieceListVertical(Color.red, new Color32(243, 32, 74, 255),  boardMain);

        //Make the "black" pieces
        p3Pieces = CreatePieceListHorizontal(Color.black, new Color32(14, 9, 60, 255), boardMain);

        //Make the "blue" pieces
        p4Pieces = CreatePieceListVertical(Color.blue, new Color32(51, 51, 204, 255), boardMain);

        //Place Pieces
        PlacePiecesHorizontal(12, 13, p3Pieces, boardMain); //White
        PlacePiecesVertical(1, 0, p2Pieces, boardMain); //Red
        PlacePiecesHorizontal(1, 0, p1Pieces, boardMain); //Black
        PlacePiecesVertical(12, 13, p4Pieces, boardMain); //Blue
         

        //Set the first turn...? (Possible break of network safety here) <-- Move this
        nextTurn(Color.blue);
    }

    //Places the horizontal pieces, THE ONES THAT GO UP AND DOWN
    private List<BasePiece> CreatePieceListHorizontal(Color tColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieceList = new List<BasePiece>();

        foreach (string newKey in placementOrderHorizontal)
        {
            //Get the piece type from order list and make it the type of the new piece
            string key = newKey;
            Type pieceType = pieceLibrary[key];

            //Store new piece
            //This will fail across networks unless it's called at the start so there is NO new game
            BasePiece pieceMarker = CreatePiece(pieceType); 
            newPieceList.Add(pieceMarker);

            //Set up the piece
            pieceMarker.Setup(tColor, spriteColor, this); //This is now self-dependent
        }

        return newPieceList;
    }

    //Places the vertical pieces, THE ONES THAT GO SIDE TO SIDE
    private List<BasePiece> CreatePieceListVertical(Color tColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieceList = new List<BasePiece>();

        for (int i = 0; i < placementOrderVertical.Length; i++)
        {
            //Get the piece type from order list and make it the type of the new piece
            string key = placementOrderVertical[i];
            Type pieceType = pieceLibrary[key];

            //Store new piece
            //This will fail across networks unless it's called at the start so there is NO new game
            BasePiece pieceMarker = CreatePiece(pieceType);
            newPieceList.Add(pieceMarker);

            //Set up the piece
            pieceMarker.Setup(tColor, spriteColor, this); //This is now self-dependent
        }

        return newPieceList;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        //Create new piece and make the piece a child of "this"
        GameObject newPiece = Instantiate(PieceTemplate);
        newPiece.transform.SetParent(transform);

        //Set scale and position
        newPiece.transform.localScale = new Vector3(1, 1, 1); //God please tell me this will scale to 30 X 30
        newPiece.transform.localRotation = Quaternion.identity; //Spin to upright

        BasePiece pieceMarker = (BasePiece) newPiece.AddComponent(pieceType);

        return pieceMarker;
    }

    /// <summary>
    /// Simple logic, the horizontal placement function utilizes board rows, while the vertical one utilizes board columns
    /// Remember, the board has the corners cu out so this will affect the STARTING points of the spawns
    /// The end points are safe by virtue of piece counts, where there is never more than 8 pieces per row/col for each player
    /// </summary>
    private void PlacePiecesHorizontal(int pawnRow, int officerRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 3; i < 11; i++)
        {
            //Place Pawns
            pieces[i - 3].Place(board.TileBoard[i, pawnRow]);
            //Debug.Log("Placing Piece at" + board.TileBoard[i, pawnRow].getBoardPos());

            //Place pieces
            pieces[i + 5].Place(board.TileBoard[i, officerRow]); //+8 to skip pawn row
            //Debug.Log("Placing Piece at" + board.TileBoard[i, officerRow].getBoardPos());
        }
    }

    private void PlacePiecesVertical(int pawnCol, int officerCol, List<BasePiece> pieces, Board board)
    {
        for (int i = 3; i < 11; i++)
        {
            //Place Pawns
            pieces[i - 3].Place(board.TileBoard[pawnCol, i]);

            //Place pieces
            pieces[i + 5].Place(board.TileBoard[officerCol, i]); //+8 to skip pawn row in placement array
        }
    }

    //Functions to maintain turn order
    //Set interactivity of pieces
    private void setInteractive(List<BasePiece> pieces, bool flag)
    {
        foreach (BasePiece piece in pieces)
        {
            piece.enabled = flag;
        }
    }

    public void nextTurn(Color color)
    {
        //If there is only one king left on the board... or somehow less
        if (allKingsAlive <= 1)
        {
            //Reset board state
            ResetPieces();

            //Revive all kings
            allKingsAlive = 4;

            //Set color to p4's color so that P1 goes first
            color = Color.blue;
        }

        Color nextColor = Color.clear;
        //Set interactivity to default pieces
        if (color == Color.blue)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.white;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 1";
            nextColor = Color.white;
            setInteractive(p1Pieces, true);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }
        else if (color == Color.white)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 2";
            nextColor = Color.red;
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, true);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }
        else if(color == Color.red)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.black;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 3";
            nextColor = Color.black;
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, true);
            setInteractive(p4Pieces, false);
        }
        else if (color == Color.black)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.blue;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 4";
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

    //Reset all players pieces so the game can begin again
    private void ResetPieces()
    {
        //Kill all promoted pieces
        foreach (BasePiece piece in promotedPieces)
        {
            piece.Kill();
            Destroy(piece);
        }

        promotedPieces.Clear();

        //Consider using a normal for loop and looping through all 4 at the same time? <-- no U wU <-- DO IT >A <
        foreach (BasePiece piece in p1Pieces)
        {
            piece.Restart();
        }

        foreach (BasePiece piece in p2Pieces)
        {
            piece.Restart();
        }

        foreach (BasePiece piece in p3Pieces)
        {
            piece.Restart();
        }

        foreach (BasePiece piece in p4Pieces)
        {
            piece.Restart();
        }
    }

    //Promote a pawn to a piece of the same color
    public void PromoteToPiece(PawnPiece pawn, Tile promotionSquare, Color team, Color actual)
    {
        //Kill pawn
        pawn.Kill();

        //TODO:Spawn possible pieces to promote to UI and move further functionality there
        //Create piece at the place we killed the pawn
        BasePiece promotedPiece = CreatePiece(typeof(QueenPiece));
        promotedPiece.Setup(team, actual, this);
        promotedPiece.Place(promotionSquare);

        //Add the new piece to the promoted piece list
        promotedPieces.Add(promotedPiece);
    }

    public void PromoteToPiece(vPawnPiece pawn, Tile promotionSquare, Color team, Color actual)
    {
        //Kill pawn
        pawn.Kill();

        //TODO:Spawn possible pieces to promote to UI and move further functionality there
        //Create piece at the place we killed the pawn
        BasePiece promotedPiece = CreatePiece(typeof(QueenPiece));
        promotedPiece.Setup(team, actual, this);
        promotedPiece.Place(promotionSquare);

        //Add the new piece to the promoted piece list
        promotedPieces.Add(promotedPiece);
    }
}
