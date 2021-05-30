using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerBoard : Board
{
    private void Start()
    {
        this.CreateBoard();
        this.Setup(this);
    }
}
