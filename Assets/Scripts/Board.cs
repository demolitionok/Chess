using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board
{
    public readonly int xSize;
    public readonly int ySize;
    private GameObject[,] CellsGameObjects;

    public Board(int ySize, int xSize)
    {
        this.ySize = ySize;
        this.xSize = xSize;
        CellsGameObjects = new GameObject[ySize, xSize];
    }

    public void RegisterCell(GameObject cellGameObject, (int, int) coords, OnTrySelectCoords onTrySelectCoords)//!coords should be written in (y, x) format
    {
        var cell = cellGameObject.AddComponent<Cell>();
        cellGameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            onTrySelectCoords.Invoke(coords);
        });
        CellsGameObjects[coords.Item1, coords.Item2] = cellGameObject;
    }
    
    public Cell GetCellByCoords((int,int) coords)//!coords should be written in (y, x) format
    {
        return CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
    }
    
}