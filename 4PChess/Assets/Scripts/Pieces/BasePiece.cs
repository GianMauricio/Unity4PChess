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
    protected PieceManager Manager;

    //Variables to be used for events and movement
    protected Vector3Int Movement = Vector3Int.one;
    protected List<Tile> legalMovesList = new List<Tile>();

    //Set up piece values
    public virtual void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
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
    public void Place(Tile newTile)
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
    private void makePath(int xDir, int yDir, int movement)
    {
        //Current position
        int currX = currTile.BoardPos.x;
        int currY = currTile.BoardPos.y;

        //Check each cell
        for (int i = 1; i <= movement; i++)
        {
            currX += xDir;
            currY += yDir;

            //TODO: Get state of currently inspected cell

            //Add inspected cell to list
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


    //Event based functions, Hijack these when attaching network functionality (feed pointer data)
    public override void OnBeginDrag(PointerEventData eventData)
    {
        //This does nothing right now lmao
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
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //Still does nothing
        base.OnEndDrag(eventData);

        //Remove legal moves list and reset the board state
        ClearPath();
    }
}
