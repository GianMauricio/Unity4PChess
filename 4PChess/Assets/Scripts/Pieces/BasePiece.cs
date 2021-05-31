using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This is an abstract class that is used as a basis to render and create pieces 
/// </summary>
public class BasePiece : EventTrigger
{
    [HideInInspector] 
    public Color defColor = Color.clear;

    protected Tile startTile = null;
    protected Tile currTile = null;

    protected RectTransform pieceRect;
    protected Board Manager;

    //Variables to be used for events and movement
    protected Vector3Int Movement = Vector3Int.one;
    protected List<Tile> legalMovesList = new List<Tile>();
    protected Tile targetTile = null;

    //Move counter (for advance and for promotion and for castling)
    public int moveCount = 0;

    //Set up piece values
    public virtual void Setup(Color teamColor, Color32 newSpriteColor, Board newPieceManager)
    {
        Manager = newPieceManager;

        //Set team color
        defColor = teamColor;

        //Set actual color (might change depending on eventual image set)
        GetComponent<Image>().color = newSpriteColor;

        //Get rect transform
        pieceRect = GetComponent<RectTransform>();
    }

    //Place piece onto the board for the first time
    public virtual void Place(Tile newTile)
    {
        //Get tile values
        currTile = newTile;
        startTile = newTile;

        //Set self to tile
        currTile.currPiece = this;

        //Align to tile and show self
        transform.position = currTile.transform.position;
        gameObject.SetActive(true);
    }

    //Create piece path
    protected void makePath(int xDir, int yDir, int movement)
    {
        //Current position
        int currX = currTile.BoardPos.x;
        int currY = currTile.BoardPos.y;

        //Check each cell
        for (int i = 1; i <= movement; i++)
        {
            currX += xDir;
            currY += yDir;

            //Add inspected cell to list if the tile is not a corner tile
            TileState state = TileState.NONE;
            state = currTile.BoardParent.ValidateCell(currX, currY, this);

            //If cell is enemy, add to list, prohibit checking of cells past this one
            if (state == TileState.ENEMY)
            {
                legalMovesList.Add(currTile.BoardParent.TileBoard[currX, currY]);
                break;
            }
            
            //If cell is not free (Forbidden, or friendly), do not add to list, prohibit further checking
            if (state != TileState.FREE)
            {
                break;
            }

            //Otherwise, add to list
            legalMovesList.Add(currTile.BoardParent.TileBoard[currX, currY]);
        }
    }

    //Override this in each subclass of piece or it WILL move like a god
    //This is not the most efficient way to implement this... TOO BAD!
    protected virtual void CheckPath()
    {
        //Horizontal Pathing
        makePath(1, 0, Movement.x); //Right
        makePath(-1, 0, Movement.x); //Left

        //Vertical Pathing
        makePath(0, 1, Movement.y); //Up
        makePath(0, -1, Movement.y); //Down

        //Upper Diagonals
        makePath(1, 1, Movement.z); //Right-facing
        makePath(-1, 1, Movement.z); //Left-facing

        //Lower Diagonals
        makePath(-1, -1, Movement.z); //Left-facing
        makePath(1, -1, Movement.z); //Right-facing
    }

    protected void ShowPath()
    {
        //Show legal moves of piece
        foreach (Tile tile in legalMovesList)
        {
            tile.OutLineImage.enabled = true;
        }
    }

    protected void ClearPath()
    {
        //Clear board of shown legal moves
        foreach (Tile tile in legalMovesList)
        {
            tile.OutLineImage.enabled = false;
        }

        //Remove all moves in legal moves list
        legalMovesList.Clear();
    }

    //General piece functions
    //Kill this piece
    public virtual void Kill()
    {
        //Clear current Tile
        currTile.currPiece = null;

        //Set self to inactive
        //TODO: Make a "Return to player holder" type behavior here
        gameObject.SetActive(false);
    }

    //Reset this piece
    public void Restart()
    {
        //Kill from where ever the fuck it is 
        Kill();

        //Set position back to start
        Place(startTile);

        //Reset move count
        moveCount = 0;
    }

    protected virtual void Move()
    {
        //If there is an enemy piece here, remove it
        targetTile.RemovePiece();

        //Clear the current tile, as we are leaving it
        currTile.currPiece = null;

        //Switch cells into the newly found cell
        currTile = targetTile;
        currTile.currPiece = this;

        //Reflect changes on actual board
        transform.position = currTile.transform.position;
        targetTile = null;
    }

    public void InvokedMove(Tile newTargetTile)
    {
        this.targetTile = newTargetTile;


        //Debug.Log("Target tile is at: " + targetTile.BoardPos);
        Move();
    }

    //Event based functions, Hijack these when attaching network functionality (feed pointer data)
    public override void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log(defColor);
        base.OnBeginDrag(eventData);

        //First check for piece path
        CheckPath();

        //Then show path
        ShowPath();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //Now this does something, important --> location data
        base.OnDrag(eventData);

        transform.position += (Vector3) eventData.delta; //Unneeded cast to vector3? <-- LIES!

        //Check if the piece is overlapping a legal tile
        foreach (Tile tile in legalMovesList) //For ever legal tile...
        {
            //Check if mouse is within the tile
            if (RectTransformUtility.RectangleContainsScreenPoint(tile.RTrans, Input.mousePosition))
            {
                //If yes, get the tile and break the loop as we have found the target
                targetTile = tile; //Fragile as HELL change this 
                break;
            }

            //Otherwise, the player is stupid and doesn't know how chess works
            targetTile = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //Still does nothing
        base.OnEndDrag(eventData);

        //Remove legal moves list and reset the board state
        ClearPath();

        //Move piece, otherwise return it to start position
        if (targetTile == null)
        {
            transform.position = currTile.gameObject.transform.position;
            return;
        }

        //Move piece
        Vector2 originCoords = new Vector2(currTile.BoardPos.x, currTile.BoardPos.y);
        Vector2 destCoords = new Vector2(targetTile.BoardPos.x, targetTile.BoardPos.y);

        currTile.BoardParent.UpdateBoards(originCoords, destCoords);

        int player = 0;
        //Make the turn change
        if (defColor == Color.white)
        {
            player = 1;
        }

        else if (defColor == Color.red)
        {
            player = 2;
        }

        else if (defColor == Color.black)
        {
            player = 3;
        }

        else if (defColor == Color.blue)
        {
            player = 4;
        }

        Manager.nextTurn(player);
    }
}
