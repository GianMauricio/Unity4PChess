using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        CreateBoard();
    }
}
