using UnityEngine;
using UnityEngine.UI;

public class KnightPiece : BasePiece
{
    public override void Setup(Color teamColor, Color32 newSpriteColor, Board newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Knight Unique stuff
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Knight");
    }

    private void makePath(int pivot)
    {
        //Target tile
        int currX = currTile.BoardPos.x;
        int currY = currTile.BoardPos.y;

        //Left L
        checkTile(currX - 2, currY + (1 * pivot));

        //UpperLeft L
        checkTile(currX - 1, currY + (2 * pivot));

        //Upper Right L
        checkTile(currX + 1, currY + (2 * pivot));

        //Right L
        checkTile(currX + 2, currY + (1 * pivot));
    }
    
    //Unique knight path
    protected override void CheckPath()
    {
        //Draw upper L's
        makePath(1);

        //Draw Lower L's
        makePath(-1);
    }

    //Check if tile is free / enemy occupied
    private void checkTile(int targetX, int targetY)
    {
        TileState state = TileState.NONE;
        state = currTile.BoardParent.ValidateCell(targetX, targetY, this); //Check if tile is valid

        //If state of tile is free or an enemy
        if (state != TileState.FRIEND && state != TileState.FORBIDDEN)
        {
            //Add tile to legal moves
            legalMovesList.Add(currTile.BoardParent.TileBoard[targetX, targetY]);
        }
    }
}
