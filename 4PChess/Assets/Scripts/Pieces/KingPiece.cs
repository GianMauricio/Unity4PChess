using UnityEngine;
using UnityEngine.UI;

public class KingPiece : BasePiece
{
    public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base Setup
        base.Setup(teamColor, newSpriteColor, newPieceManager);

        //Set rook unique shit
        Movement = new Vector3Int(1, 1, 1); //Scan board columns and diagonals but only 1 square to each
        GetComponent<Image>().sprite = Resources.Load<Sprite>("t_King");
    }

    public override void Kill()
    {
        base.Kill();//Still kill self

        //But also reduce total kings alive
        Manager.allKingsAlive--;
    }
}
