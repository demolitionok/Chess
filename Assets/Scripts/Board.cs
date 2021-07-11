using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject CellPrefab;
    private float cellWidth;
    private float cellHeight;
    public float Offset;


    private void GenerateBoard()
    {
        var xSize = GameController.CellsGameObjects.GetLength(1);
        var ySize = GameController.CellsGameObjects.GetLength(0);
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                GameObject cellGameObject = Instantiate(CellPrefab,
                    new Vector3(x * cellWidth + Offset, -y * cellHeight - Offset), Quaternion.identity);
                cellGameObject.transform.SetParent(gameObject.transform, false);

                CellFabric.RegisterCell(cellGameObject, (y, x));

                cellGameObject = GameController.CellsGameObjects[y, x];

                var cell = cellGameObject.GetComponent<Cell>();
                cell.State = null;
                if (y <= 1)
                {
                    cell.State = Side.Black;
                }
                if (y >= 6)
                {
                    cell.State = Side.White;
                }

                if (y == 1 || y == 6)
                {
                    cell.Figure = new Pawn(
                        "pawn",
                        cell.State == Side.White ? Resources.Load<Sprite>("wpawn") : Resources.Load<Sprite>("bpawn")
                    );
                }

                if (
                    y == 0 && (x == 0 || x == xSize - 1)
                    ||
                    y == ySize - 1 && (x == 0 || x == xSize - 1)
                )
                {
                    cell.Figure = new Tower(
                        "tower",
                        cell.State == Side.White ? Resources.Load<Sprite>("wtower") : Resources.Load<Sprite>("btower")
                    );
                }
                if (
                    y == 0 && (x == 2 || x == xSize - 3)
                    ||
                    y == ySize - 1 && (x == 2 || x == xSize - 3)
                )
                {
                    cell.Figure = new Bishop(
                        "bishop",
                        cell.State == Side.White ? Resources.Load<Sprite>("wbishop") : Resources.Load<Sprite>("bbishop")
                    );
                }
                if (
                    (y == 0 || y == ySize - 1) && (x == 3 || x == xSize - 5)
                )
                {
                    cell.Figure = new Queen(
                        "queen",
                        cell.State == Side.White ? Resources.Load<Sprite>("wqueen") : Resources.Load<Sprite>("bqueen")
                    );
                }
                if (
                    y == 0 && (x == 1 || x == xSize - 2)
                    ||
                    y == ySize - 1 && (x == 1 || x == xSize - 2)
                )
                {
                    cell.Figure = new Knight(
                        "knight",
                        cell.State == Side.White ? Resources.Load<Sprite>("wknight") : Resources.Load<Sprite>("bknight")
                    );
                }
                if (
                    (y == 0 || y == ySize - 1) && (x == 4 || x == xSize - 4)
                )
                {
                    cell.Figure = new King(
                        "king",
                        cell.State == Side.White ? Resources.Load<Sprite>("wking") : Resources.Load<Sprite>("bking")
                    );
                }
            }
        }
    }


    void Start()
    {
        GameController.OnTrySelectCoords += GameController.TrySelectCoords;
        cellWidth = CellPrefab.GetComponent<RectTransform>().rect.width;
        cellHeight = CellPrefab.GetComponent<RectTransform>().rect.height;
        GenerateBoard();
        GameController.UpdateBoard();
    }

    void Update()
    {
    }
}