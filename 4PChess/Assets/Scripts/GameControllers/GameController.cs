using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public enum GameState {Init, inPlay, Finished}
public abstract class GameController : MonoBehaviour
{
    protected GameState currGameState;
    private Board actualBoard;
    private UIManagement uimanager;
    protected Player P1;
    protected Player P2;
    protected Player P3;
    protected Player P4;

    protected abstract void SetGameState(GameState newState);
    public abstract void TryStartGame();

    public void setDependencies(UIManagement UI, Board newBoard)
    {
        this.actualBoard = newBoard;
        uimanager = UI;
    }
}
