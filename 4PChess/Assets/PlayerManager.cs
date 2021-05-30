using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Board activeBoard;

    void Start()
    {
        //Create the board
        activeBoard.CreateBoard();

        //Create the Pieces
        activeBoard.Setup(activeBoard);
    }
}
