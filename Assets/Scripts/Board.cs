using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board
{
    private readonly int xSize;
    private readonly int ySize;
    private GameObject[,] CellsGameObjects;

    public Board(int ySize, int xSize)
    {
        this.ySize = ySize;
        this.xSize = xSize;
        CellsGameObjects = new GameObject[ySize, xSize];
    }

    public void RegisterCell(GameObject cellGameObject, (int, int) coords, ActionSend actionSend) //!coords should be written in (y, x) format
    {
        var cell = cellGameObject.AddComponent<Cell>();
        cellGameObject.GetComponent<Button>().onClick.AddListener(() => { actionSend.Invoke(coords); });
        CellsGameObjects[coords.Item1, coords.Item2] = cellGameObject;
    }

    public Cell GetCellByCoords((int, int) coords) //!coords should be written in (y, x) format
    {
        return CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
    }
}