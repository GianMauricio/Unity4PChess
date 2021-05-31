using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Enum for tile states
public enum TileState
{
    NONE = 0,
    FRIEND = 1,
    ENEMY = 2,
    FREE = 3,
    FORBIDDEN = 4
}

public abstract class Board : MonoBehaviour
{
    private static int boardDimensions = 14;
    public GameObject TileTemplate;

    [HideInInspector] public Tile[,] TileBoard = new Tile[boardDimensions, boardDimensions];

    protected virtual void Awake()
    {
        this.CreateBoard();
        this.Setup(this);
    }

    public void CreateBoard()
    {
        //Render all tiles
        for (int y = 0; y < boardDimensions; y++)
        {
            for (int x = 0; x < boardDimensions; x++)
            {
                //Create tile at the center of this transform
                GameObject newTile = Instantiate(TileTemplate, transform);
                newTile.transform.SetParent(transform);

                //Position the tile according to location in board
                RectTransform tileRect = newTile.GetComponent<RectTransform>();
                tileRect.anchoredPosition = new Vector2((x * 45) + 50, (y * 45) + 50);

                //Setup tile as a board element
                TileBoard[x, y] = newTile.GetComponent<Tile>();
                TileBoard[x, y].Setup(new Vector2Int(x, y), this);
            }
        }

        //Set tile colors
        for (int y = 0; y < boardDimensions; y++)
        {
            for (int x = 0; x < boardDimensions; x++)
            {
                if ((x + y) % 2 == 1)
                {
                    TileBoard[x, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
                }
            }
        }

        //Delete corners of board
        for (int y = 0; y < boardDimensions; y++)
        {
            if (y < 3 || y > 10)
            {
                for (int x = 0; x < boardDimensions; x++)
                {
                    if (x < 3 || x > 10)
                    {
                        TileBoard[x, y].setActive(false);
                    }
                }
            }
        }
    }

    //Validate cells for query
    public TileState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {
        //If OOB check...No
        if (targetX < 0 || targetX > 13) return TileState.FORBIDDEN; //If it exits board dimensions laterally
        if (targetY < 0 || targetY > 13) return TileState.FORBIDDEN; //If it exits board dimensions longitudinally
        
        //Get cell data
        Tile targetTile = TileBoard[targetX, targetY];

        //Check if cell is a corner cell...No
        if (!targetTile.gameObject.activeInHierarchy) return TileState.FORBIDDEN; //If the cell is a corner cell

        //If the cell has a piece on it...
        if (targetTile.currPiece != null)
        {
            //If the piece is friendly
            if (checkingPiece.defColor == targetTile.currPiece.defColor) return TileState.FRIEND;

            //If the piece wants blood
            if (checkingPiece.defColor != targetTile.currPiece.defColor) return TileState.ENEMY;

            //TODO: Check for null if possible or required to avoid crashes
        }

        //Otherwise return free
        return TileState.FREE;
    }

    public GameObject PieceTemplate;
    

    [HideInInspector]
    public int allKingsAlive = 4; //Track whether someone has died yet

    //Piece arrays
    protected List<BasePiece> p1Pieces = null; //Up --> Down
    protected List<BasePiece> p2Pieces = null; //Down --> Up
    protected List<BasePiece> p3Pieces = null; //Left --> Right
    protected List<BasePiece> p4Pieces = null; //Right --> Left

    protected List<BasePiece> promotedPieces = new List<BasePiece>();

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
        p2Pieces = CreatePieceListVertical(Color.red, new Color32(243, 32, 74, 255), boardMain);

        //Make the "black" pieces
        p3Pieces = CreatePieceListHorizontal(Color.black, new Color32(14, 9, 60, 255), boardMain);

        //Make the "blue" pieces
        p4Pieces = CreatePieceListVertical(Color.blue, new Color32(51, 51, 204, 255), boardMain);

        //Place Pieces
        PlacePiecesHorizontal(12, 13, p3Pieces, boardMain); //White
        PlacePiecesVertical(1, 0, p2Pieces, boardMain); //Red
        PlacePiecesHorizontal(1, 0, p1Pieces, boardMain); //Black
        PlacePiecesVertical(12, 13, p4Pieces, boardMain); //Blue
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

        BasePiece pieceMarker = (BasePiece)newPiece.AddComponent(pieceType);

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
    protected void setInteractive(List<BasePiece> pieces, bool flag)
    {
        foreach (BasePiece piece in pieces)
        {
            piece.enabled = flag;
        }
    }

    public virtual void nextTurn(int Player)
    {
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

        //Create piece at the place we killed the pawn
        BasePiece promotedPiece = CreatePiece(typeof(QueenPiece));
        promotedPiece.Setup(team, actual, this);
        promotedPiece.Place(promotionSquare);

        //Add the new piece to the promoted piece list
        promotedPieces.Add(promotedPiece);
    }

    //Simple logic: All you need to update a board is to have a piece origin square and a destination square,
    //then a call to add a move to the pieces' move counter. 

    //NOTE: This function has NO IDEA if it will work, so it has to take in data and pray
    public virtual void UpdateBoards(Vector2 originTile, Vector2 destinationTile)
    {
        Vector2Int OriginCoords = new Vector2Int(Mathf.RoundToInt(originTile.x), Mathf.RoundToInt(originTile.y));
        Vector2Int DestCoords = new Vector2Int(Mathf.RoundToInt(destinationTile.x), Mathf.RoundToInt(destinationTile.y));

        //Get the old tile which the piece WAS on
        Tile oldTile = TileBoard[OriginCoords.x, OriginCoords.y];

        //Move that piece to the tile where it is supposed to be
        Tile newTile = TileBoard[DestCoords.x, DestCoords.y];

        //Invoke movement from piece at origin tile and tell it to move to destination tile. And pray. To all gods
        BasePiece invokedPiece = oldTile.currPiece;
        invokedPiece.InvokedMove(newTile);
    }
}
