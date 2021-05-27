using UnityEngine;
using UnityEngine.UI;

public class QueenPiece : BasePiece
{
    public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set Mu'h Queen <-- Obligatory Muh Queen reference, very good
        Movement = new Vector3Int(13, 13, 13); //Scan board columns and diagonals
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_Queen");
    }
}
