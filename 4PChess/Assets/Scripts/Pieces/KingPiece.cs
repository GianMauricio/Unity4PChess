using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class KingPiece : BasePiece
{
    private RookPiece leftRook = null;
    private RookPiece rightRook = null;

    private RookPiece upRook = null;
    private RookPiece downRook = null;

    public override void Setup(Color teamColor, Color32 newSpriteColor, Board newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set rook unique shit
        Movement = new Vector3Int(1, 1, 1); //Scan board columns and diagonals but only 1 square to each
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_King");
    }

    public override void Kill()
    {
        base.Kill();//Still kill self

        //But also reduce total kings alive
        Manager.allKingsAlive--;
    }

    protected override void CheckPath()
    {
        base.CheckPath();

        //If the kings are horizontal
        if (defColor == Color.white || defColor == Color.black)
        {
            //Check for Castle possibilities
            rightRook = GetRook(1, 4);
            leftRook = GetRook(-1, 3);
        }

        //If the kings are vertical
        else if (defColor == Color.red || defColor == Color.blue)
        {
            //Check for Castle possibilities
            upRook = GetRookV(1, 4); 
            downRook = GetRookV(-1, 3);
        }
    }

    protected override void Move()
    {
        base.Move();

        //If the kings are horizontal
        if (defColor == Color.white || defColor == Color.black)
        {
            //If the move was a castle, the move the rook as well
            if (canCastle(leftRook))
            {
                leftRook.Castle();
            }

            if (canCastle(rightRook))
            {
                rightRook.Castle();
            }
        }
        
        //If the kings are vertical
        else if (defColor == Color.red || defColor == Color.blue)
        {
            //If the move was a castle, the move the rook as well
            if (canCastle(upRook))
            {
                upRook.Castle();
            }

            if (canCastle(downRook))
            {
                downRook.Castle();
            }
        }
    }

    //Finds out if the king can castle
    private bool canCastle(RookPiece targetRook)
    {
        if (targetRook == null)
        {
            return false;
        }

        if (targetRook.CastleTriggerTile != currTile)
        {
            return false;
        }

        return true;
    }

    //Returns a requested rook HORIZONTALLY
    private RookPiece GetRook(int dir, int count)
    {
        //If king has moved
        if (moveCount > 0)
        {
            return null; //No castles for you
        }

        //Get position of the king 
        int actualX = startTile.BoardPos.x;
        int actualY = startTile.BoardPos.y;

        //Check all tiles in between the king and rook
        for (int i = 1; i < count; i++) //For all tiles in bet rook and king
        {
            int offsetX = actualX + (dir * i);
            if (currTile.BoardParent.ValidateCell(offsetX, actualY, this) != TileState.FREE) //If cell is otherwise occupied...
            {
                return null; //No castles for you
            }
        }

        //Try to petition rook
        Tile rookTile = currTile.BoardParent.TileBoard[actualX + (count * dir), actualY];
        RookPiece targetRook = null;

        //Cast rook as rook
        if (rookTile.currPiece is RookPiece piece)
        {
            targetRook = piece;
        }
        else return null; //If not, no castles for you!

        //Check if rook as moved
        if (targetRook.moveCount > 0) //If rook has moved
        {
            return null; // No castles for you!
        }

        //Check if rook is ours
        if (targetRook.defColor != defColor)
        {
            return null; //No castles for you!
        }

        //If all pass,
        //Add trigger cell to path movement
        legalMovesList.Add(targetRook.CastleTriggerTile);
        return targetRook;
    }

    private RookPiece GetRookV(int dir, int count)
    {
        //If king has moved
        if (moveCount > 0)
        {
            return null; //No castles for you
        }

        //Get position of the king 
        int actualX = startTile.BoardPos.x;
        int actualY = startTile.BoardPos.y;

        //Check all tiles in between the king and rook
        for (int i = 1; i < count; i++) //For all tiles in bet rook and king
        {
            int offsetY = actualY + (dir * i);

            if (currTile.BoardParent.ValidateCell(actualX, offsetY, this) != TileState.FREE) //If cell is otherwise occupied...
            {
                return null; //No castles for you
            }
        }

        //Try to petition rook
        Tile rookTile = currTile.BoardParent.TileBoard[actualX , actualY + (count * dir)];
        RookPiece targetRook = null;

        //Cast rook as rook
        if (rookTile.currPiece is RookPiece piece)
        {
            targetRook = piece;
        }
        else return null; //If not, no castles for you!

        //Check if rook as moved
        if (targetRook.moveCount > 0) //If rook has moved
        {
            return null; // No castles for you!
        }

        //Check if rook is ours
        if (targetRook.defColor != defColor)
        {
            return null; //No castles for you!
        }

        //If all pass,
        //Add trigger cell to path movement
        legalMovesList.Add(targetRook.CastleTriggerTile);
        return targetRook;
    }
}
