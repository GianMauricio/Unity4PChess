using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will handle all logic in relation to a single Tile
/// </summary>
public class Tile : MonoBehaviour
{
    public Image OutLineImage;

    [HideInInspector] 
    public Vector2Int BoardPos = Vector2Int.zero;
    public Board BoardParent;
    public RectTransform RTrans = null;
    public bool isActive = true;

    //Piece data
    public BasePiece currPiece;

    public void Setup(Vector2Int newBoardPos, Board newBoard)
    {
        BoardPos = newBoardPos;
        BoardParent = newBoard;

        RTrans = this.GetComponent<RectTransform>();
    }

    //Return the tiles position on the board coords system
    public Vector2Int getBoardPos()
    {
        return BoardPos;
    }

    //Sets the tile as active or not used for piece-type functions
    public void setActive(bool flag)
    {
        isActive = flag;

        if (!isActive)
        {
            this.transform.GetComponent<Image>().color = Color.clear;
            OutLineImage.GetComponent<Image>().color = Color.clear;
        }
    }

    //Piece-type functions

    //Remove piece from tile
    public void RemovePiece()
    {
        if (currPiece != null)
        {
            currPiece.Kill();
        }
    }
}
