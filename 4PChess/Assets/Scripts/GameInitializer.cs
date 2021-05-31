using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent object")]
    public SPGameController singlePlayerChessControllerPrefab;
    public MPGameController multiPlayerChessControllerPrefab;
    public SinglePlayerBoard localBoardPrefab;
    public MultiplayerBoard remoteBoardPrefab;

    [Header("Scene reference")]
    public NetworkSocket netManager;
    public UIManagement uiManager;
    public Transform boardparent; //CANVAS PARENT 

    public void CreateMultiplayerBoard()
    {
        //If you are the first player to enter
        if (netManager.getPlayersInRoom() == 1)
        {
            GameObject multiBoard = PhotonNetwork.Instantiate(remoteBoardPrefab.name, Vector3.zero, 
                boardparent.rotation);

            //Should ensure that the board actually renders
            multiBoard.transform.SetParent(boardparent);
        }
    }

    public void CreateSinglePlayerBoard()
    {
        GameObject boardObj = Instantiate(localBoardPrefab).gameObject;
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindObjectOfType<MultiplayerBoard>();
        if (board)
        {
            MPGameController controller = Instantiate(multiPlayerChessControllerPrefab);
        }
    }
}
