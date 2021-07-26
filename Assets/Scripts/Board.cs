using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board
{
    private readonly int xSize;
    private readonly int ySize;
    private Cell[,] Cells;

    public Board(int ySize, int xSize)
    {
        this.ySize = ySize;
        this.xSize = xSize;
        Cells = new Cell[ySize, xSize];
    }

    public void RegisterCell(Cell cell, (int, int) coords, ActionSend actionSend) //!coords should be written in (y, x) format
    {
        var cellGameObject = cell.gameObject;
        cellGameObject.GetComponent<Button>().onClick.AddListener(() => { actionSend.Invoke(coords); });
        Cells[coords.Item1, coords.Item2] = cell;
    }

    public Cell GetCellByCoords((int, int) coords) //!coords should be written in (y, x) format
    {
        return Cells[coords.Item1, coords.Item2];
    }
}