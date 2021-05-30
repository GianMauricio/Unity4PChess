using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Init, inPlay, Finished}
public abstract class GameController : MonoBehaviour
{
    protected GameState currGameState;

    protected abstract void SetGameState(GameState newState);
    public abstract void TryStartGame();
}
