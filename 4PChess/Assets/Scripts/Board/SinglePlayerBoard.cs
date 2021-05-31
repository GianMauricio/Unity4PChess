using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SinglePlayerBoard : Board
{
    public GameObject turnUI;

    protected override void Awake()
    {
        base.Awake();

        //This is bad implementation... Too bad!
        turnUI = GameObject.Find("TurnUI");
    }

    public override void nextTurn(int Player)
    {
        base.nextTurn(Player);

        Color color = Color.clear;
        if (Player == 1)
        {
            color = Color.white;
        }

        else if (Player == 2)
        {
            color = Color.red;
        }

        else if (Player == 3)
        {
            color = Color.black;
        }

        else if (Player == 4)
        {
            color = Color.blue;
        }

        //Change own UI colors
        if (color == Color.blue)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.white;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 1";
        }
        else if (color == Color.white)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 2";
        }
        else if (color == Color.red)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.black;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 3";
        }
        else if (color == Color.black)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.blue;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 4";
        }

        //Change own UI colors
        if (color == Color.blue)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.white;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 1";
        }
        else if (color == Color.white)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.red;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 2";
        }
        else if (color == Color.red)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.black;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 3";
        }
        else if (color == Color.black)
        {
            turnUI.GetComponent<TextMeshProUGUI>().color = Color.blue;
            turnUI.GetComponent<TextMeshProUGUI>().text = "Player 4";
        }
    }

    public override void UpdateBoards(Vector2 originTile, Vector2 destinationTile)
    {
        base.UpdateBoards(originTile, destinationTile);
    }
}
