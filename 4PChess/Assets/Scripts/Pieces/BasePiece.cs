using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This is an abstract class that is used as a basis to render and create pieces 
/// </summary>
public class BasePiece : MonoBehaviour
{
    [HideInInspector] 
    public Color defColor = Color.clear;

    protected Tile startTile = null;
    protected Tile currTile = null;

    protected RectTransform pieceRect;
    protected PieceManager Manager;

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
}
