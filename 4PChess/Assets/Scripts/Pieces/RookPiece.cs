using UnityEngine;
using UnityEngine.UI;

public class RookPiece : BasePiece
{
    public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set rook unique shit
        Movement = new Vector3Int(13, 13, 0); //Scan board columns
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Rook");
    }
}
