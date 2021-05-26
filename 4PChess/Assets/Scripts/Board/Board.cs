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

public class Board : MonoBehaviour
{
    private static int boardDimensions = 14;
    public GameObject TileTemplate;

    [HideInInspector] public Tile[,] TileBoard = new Tile[boardDimensions, boardDimensions];


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

        /*
        //Debug Tiles
        for (int x = 0; x < boardDimensions; x++)
        {
            for (int y = 0; y < boardDimensions; y++)
            {
                Debug.Log(TileBoard[x, y].GetComponent<Tile>().getBoardPos());
            }
        }
        */

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
}
