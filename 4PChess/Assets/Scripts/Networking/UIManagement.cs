using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    [Header("Scene Dependencies")] 
    public NetworkSocket networkManager;

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
    public GameObject gameUI; //PARENT CANVAS P A R E N T  C A N V A S!!!!!

    private void Awake()
    {
        //Do something to ensure peer parity <-- gotchu
        OnGameLaunched();
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
    }
}

