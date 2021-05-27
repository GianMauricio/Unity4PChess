using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// THIS GOES SIDE TO SIDE
/// </summary>
public class PawnPiece : BasePiece
{
    //Override base set up
    public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set pawn stuff
        //Check for side and direction
        if(defColor == Color.red) Movement = new Vector3Int(1, 0, 1); //Scan RIGHT
        if (defColor == Color.blue) Movement = new Vector3Int(-1, 0, -1); //Scan LEFT
        
        //Set sprite
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Pawn");
    }

    //Override move function
    protected override void Move()
    {
        base.Move();

        moveCount++;

        CheckForPromotion();
    }

    private void CheckForPromotion()
    {
        //Check if this pawn has traveled 8 tiles away from it's starting tile
        int distFromStart = currTile.BoardPos.x - startTile.BoardPos.x;
        if (Mathf.Abs(distFromStart) > 8)
        {
            //Sprite color
            Color actualSprite = GetComponent<Image>().color;

            Manager.PromoteToPiece(this, currTile, defColor, actualSprite);
        }
    }

    //Check if tile is free
    private bool checkTile(int targetX, int targetY, TileState targetState)
    {
        TileState state = TileState.NONE;
        state = currTile.BoardParent.ValidateCell(targetX, targetY, this); //Check if tile is valid

        //if state of tile is intended state
        if (state == targetState)
        {
            //Add tile to legal moves
            legalMovesList.Add(currTile.BoardParent.TileBoard[targetX, targetY]);
            return true;
        }
        return false;
    }

    protected override void CheckPath()
    {
        //Target position
        int currX = currTile.BoardPos.x;
        int currY = currTile.BoardPos.y;

        //Tile to top left
        checkTile(currX + Movement.z, currY - Movement.z, TileState.ENEMY);

        //Tile to top right
        checkTile(currX + Movement.z, currY + Movement.z, TileState.ENEMY);

        //Tile directly in front
        if (checkTile(currX + Movement.x, currY, TileState.FREE))
        {
            //If this is the first move that the pawn has ever made
            if (moveCount == 0)
            {
                checkTile(currX + Movement.x * 2, currY, TileState.FREE);
            }
        }
    }
}
