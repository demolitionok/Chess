using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState
{
    Empty,
    Black,
    White,
}

public class Cell : MonoBehaviour
{
    public CellState State;
    public int x;
    public int y;

    public void ChooseCell()
    {
        GameController.ChosenCell = this;
    }
}
