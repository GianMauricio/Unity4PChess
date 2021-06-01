using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour, IOnEventCallback
{
    protected const byte SET_GAME_STATE_EVENT_CODE = 1;
    protected const byte SET_GAME_WINNER = 2;

    private GameState currState;
    private int currWinner = 0;

    [Header("Scene Dependencies")] 
    public NetworkSocket networkManager;
    public MPGameController chessController;

    [Header("Buttons")] 
    public Button chooseWhite;
    public Button chooseBlack;
    public Button chooseBlue;
    public Button chooseRed;

    [Header("Texts")] 
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI connectionStatusText;

    [Header("Screens")] 
    public GameObject gameOverScreen;
    public GameObject connectScreen;
    public GameObject teamSelectScreen;
    public GameObject gameModeSelect;
    public GameObject gameUI;

    private void Awake()
    {
        //Ensures Peer parity
        OnGameLaunched();
    }

    //On Enable
    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetDependencies(MPGameController chessGame)
    {
        this.chessController = chessGame;
    }

    private void OnGameLaunched()
    {
        DisableAllScreens();
        gameModeSelect.SetActive(true);
    }

    public void onSinglePlayerModeSelected()
    {
        DisableAllScreens();
        gameUI.SetActive(true);
    }

    public void onMultiPlayerModeSelected()
    {
        connectionStatusText.gameObject.SetActive(true);
        DisableAllScreens();
        connectScreen.SetActive(true);
    }

    public void onConnectPressed()
    {
        networkManager.Connect();
    }

    //Team select screen
    public void showTeamSelect()
    {
        DisableAllScreens();
        teamSelectScreen.SetActive(true);
    }

    public void SetConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    public void RestrictTeamChoice(int teamChosen)
    {
        if (teamChosen == 1)
        {
            chooseWhite.interactable = false;
        }

        else if (teamChosen == 2)
        {
            chooseRed.interactable = false;
        }

        else if (teamChosen == 3)
        {
            chooseBlack.interactable = false;
        }

        else if (teamChosen == 4)
        {
            chooseBlue.interactable = false;
        }
    }

    //Launch into game
    public void tryStartGame()
    {
        SetGameState(GameState.inPlay);
    }

    //private functions
    private void DisableAllScreens()
    {
        gameOverScreen.SetActive(false);
        connectScreen.SetActive(false);
        teamSelectScreen.SetActive(false);
        gameUI.SetActive(false);
        gameModeSelect.SetActive(false);
    }

    public void setTeam(int selectedPlayer)
    {
        networkManager.setTeam(selectedPlayer);

        if (selectedPlayer == 1)
        {
            chessController.setLocalPlayerColor(Color.white);
        }

        else if (selectedPlayer == 2)
        {
            chessController.setLocalPlayerColor(Color.red);
        }

        else if (selectedPlayer == 3)
        {
            chessController.setLocalPlayerColor(Color.black);
        }

        else if (selectedPlayer == 4)
        {
            chessController.setLocalPlayerColor(Color.blue);
        }
    }

    protected void SetGameState(GameState newState)
    {
        object[] content = new object[] { (int)newState };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SET_GAME_STATE_EVENT_CODE, content, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("Start event sent");
    }

    //Access to self / selves
    protected void tryEndGame(int winner)
    {
        object[] content = new object[] { (int)winner };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SET_GAME_WINNER, content, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("End event sent with winner: " + winner);
    }

    //Access to all
    public void setWinnerAndEnd(int winner)
    {
        tryEndGame(winner);
    }

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("Event received");
        byte eventCode = photonEvent.Code;
        if (eventCode == SET_GAME_STATE_EVENT_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;
            GameState state = (GameState)data[0];

            this.currState = state;
            Debug.Log("State event parsed");
        }

        else if (eventCode == SET_GAME_WINNER)
        {
            object[] data = (object[])photonEvent.CustomData;
            currWinner = (int)data[0];

            this.currState = GameState.Finished;
            Debug.Log("End game event parsed");
        }

        if (currState == GameState.inPlay)
        { 
            Debug.Log("Start event passed");
            StartGame();
        }

        else if (currState == GameState.Finished)
        {
            Debug.Log("End event passed");
            EndGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        DisableAllScreens();
        gameUI.SetActive(true);
    }

    private void EndGame()
    {
        Debug.Log("Ending game");
        DisableAllScreens();
        gameOverScreen.SetActive(true);

        Debug.Log("Current winner: " + currWinner);
        //Parse winner text and color values
        if (currWinner == 1)
        {
            resultText.text = "Player 1";
            resultText.color = Color.white;
        }

        else if (currWinner == 2)
        {
            resultText.text = "Player 2";
            resultText.color = Color.red;
        }

        else if (currWinner == 3)
        {
            resultText.text = "Player 3";
            resultText.color = Color.black;
        }

        else if (currWinner == 4)
        {
            resultText.text = "Player 4";
            resultText.color = Color.blue;
        }
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom(true);
        Application.Quit();
    }
}

