using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public GameObject PieceTemplate;

    [HideInInspector] 
    public int allKingsAlive = 4; //Track whether someone has died yet

    //Piece arrays
    private List<BasePiece> p1Pieces = null; //Up --> Down
    private List<BasePiece> p2Pieces = null; //Down --> Up
    private List<BasePiece> p3Pieces = null; //Left --> Right
    private List<BasePiece> p4Pieces = null; //Right --> Left

    //Order of placement
    private string[] placementOrderHorizontal =
    {
        "hP", "hP", "hP", "hP", "hP", "hP", "hP", "hP",
        "R", "N", "B", "K", "Q", "B", "N", "R"
    };

    //Order of placement
    private string[] placementOrderVertical =
    {
        "vP", "vP", "vP", "vP", "vP", "vP", "vP", "vP",
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
        p2Pieces = CreatePieceListVertical(Color.red, new Color32(14, 9, 83, 255),  boardMain);

        //Make the "black" pieces
        p3Pieces = CreatePieceListHorizontal(Color.black, new Color32(243, 32, 74, 255), boardMain);

        //Make the "blue" pieces
        p4Pieces = CreatePieceListVertical(Color.blue, new Color32(51, 51, 204, 255), boardMain);

        //Place Pieces
        PlacePiecesHorizontal(0, 1, p1Pieces, boardMain); //White
        PlacePiecesHorizontal(12, 13, p2Pieces, boardMain); //Black
        PlacePiecesVertical(1, 0, p3Pieces, boardMain); //Blue
        PlacePiecesVertical(12, 13, p4Pieces, boardMain); //Red


        //Set the first turn...? (Possible break of network safety here) <-- Move this
        nextTurn(Color.blue);
    }

    //Places the horizontal pieces, THE ONES THAT GO UP AND DOWN
    private List<BasePiece> CreatePieceListHorizontal(Color tColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieceList = new List<BasePiece>();

        for (int i = 0; i < placementOrderHorizontal.Length; i++)
        {
            //Create new piece and make the piece a child of "this"
            GameObject newPiece = Instantiate(PieceTemplate);
            newPiece.transform.SetParent(transform);

            //Set scale and position
            newPiece.transform.localScale = new Vector3(1, 1, 1); //God please tell me this will scale to 75 X 75
            newPiece.transform.localRotation = Quaternion.identity; //Spin to upright

            //Get the piece type from order list and make it the type of the new piece
            string key = placementOrderHorizontal[i];
            Type pieceType = pieceLibrary[key];

            //Store new piece
            //This will fail across networks unless it's called at the start so there is NO new game
            BasePiece pieceMarker = (BasePiece) newPiece.AddComponent(pieceType);
            newPieceList.Add(pieceMarker);

            //Set up the piece
            pieceMarker.Setup(tColor, spriteColor, this); //This is now self-dependent
        }

        return newPieceList;
    }

    //Places the vertical pieces, THE ONES THAT SIDE TO SIDE
    private List<BasePiece> CreatePieceListVertical(Color tColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieceList = new List<BasePiece>();

        for (int i = 0; i < placementOrderVertical.Length; i++)
        {
            //Create new piece and make the piece a child of "this"
            GameObject newPiece = Instantiate(PieceTemplate);
            newPiece.transform.SetParent(transform);

            //Set scale and position
            newPiece.transform.localScale = new Vector3(1, 1, 1); //God please tell me this will scale to 75 X 75
            newPiece.transform.localRotation = Quaternion.identity; //Spin to upright

            //Get the piece type from order list and make it the type of the new piece
            string key = placementOrderVertical[i];
            Type pieceType = pieceLibrary[key];

            //Store new piece
            //This will fail across networks unless it's called at the start so there is NO new game
            BasePiece pieceMarker = (BasePiece)newPiece.AddComponent(pieceType);
            newPieceList.Add(pieceMarker);

            //Set up the piece
            pieceMarker.Setup(tColor, spriteColor, this); //This is now self-dependent
        }

        return newPieceList;
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
        //If there is only one king left on the board
        if (allKingsAlive == 1)
        {
            //Reset board state
            ResetPieces();

            //Revive all kings
            allKingsAlive = 4;

            //Set color to p4's color so that P1 goes first
            color = Color.blue;
        }


        if (color == Color.blue)
        {
            setInteractive(p1Pieces, true);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }
        else if (color == Color.white)
        {
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, true);
            setInteractive(p4Pieces, false);
        }
        else if(color == Color.red)
        {
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, false);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, true);
        }
        else if (color == Color.black)
        {
            setInteractive(p1Pieces, false);
            setInteractive(p2Pieces, true);
            setInteractive(p3Pieces, false);
            setInteractive(p4Pieces, false);
        }
    }

    //Reset all players pieces so the game can begin again
    private void ResetPieces()
    {
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
}
