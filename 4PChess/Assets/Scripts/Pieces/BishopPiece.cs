using UnityEngine;
using UnityEngine.UI;

public class BishopPiece : BasePiece
{
    public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set bishop unique shit
        Movement = new Vector3Int(0, 0, 13); //Scan board diagonally
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Bishop");
    }
}
