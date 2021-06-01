using UnityEngine;
using UnityEngine.UI;

public class RookPiece : BasePiece
{
    [HideInInspector]
    public Tile CastleTriggerTile = null;
    public Tile CastleDestinationTile = null;

    public override void Setup(Color teamColor, Color32 newSpriteColor, Board newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set rook unique stuff
        Movement = new Vector3Int(13, 13, 0); //Scan board columns
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Rook");
    }

    public override void Place(Tile newTile)
    {
        base.Place(newTile);

        //Set starting cell
        int actualX = newTile.BoardPos.x;
        int actualY = newTile.BoardPos.y;

        ///Conditions
        /*
         * 1. (3, 13) --> White
         * 2. (10, 13) --> White
         * 3. (0, 3) --> Red
         * 4. (0, 10) --> Red
         * 5. (3, 0) --> Black
         * 6. (10, 0) --> Black
         * 7. (13, 3) --> Blue
         * 8. (13, 10) --> Blue
        */

        if (actualX == 3) //Black & White Kingsides
        {
            CastleTriggerTile = currTile.BoardParent.TileBoard[actualX + 1, actualY];
            CastleDestinationTile = currTile.BoardParent.TileBoard[actualX + 2, actualY];
        }

        else if (actualX == 10) //Black & White Queensides
        {
            CastleTriggerTile = currTile.BoardParent.TileBoard[actualX - 2, actualY];
            CastleDestinationTile = currTile.BoardParent.TileBoard[actualX - 3, actualY];
        }

        else if (actualY == 10) //Red & blue Queensides
        {
            CastleTriggerTile = currTile.BoardParent.TileBoard[actualX, actualY - 2];
            CastleDestinationTile = currTile.BoardParent.TileBoard[actualX, actualY - 3];
        }

        else if (actualY == 3) // Red & Blue Kingside
        {
            CastleTriggerTile = currTile.BoardParent.TileBoard[actualX, actualY + 1];
            CastleDestinationTile = currTile.BoardParent.TileBoard[actualX, actualY + 2];
        }
    }

    public void Castle()
    {
        //Set new target cell
        targetTile = CastleDestinationTile;

        //Attempt to move to that cell
        Move();
    }
}
