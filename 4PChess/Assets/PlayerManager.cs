using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Board activeBoard;

    public PieceManager pieceManager;

    void Start()
    {
        //Create the board
        activeBoard.CreateBoard();

        //Create the Pieces
        pieceManager.Setup(activeBoard);
    }
}
