using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPGameController : GameController
{
    protected override void SetGameState(GameState newState)
    {
        this.currGameState = newState;
    }

    public override void TryStartGame()
    {
        this.currGameState = GameState.inPlay;
    }
}
