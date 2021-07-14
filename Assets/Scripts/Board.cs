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
                cell.Figure = null;
                
                if (y == 1)
                {
                    cell.Figure = new Pawn("pawn", Side.Black);
                }   
                if (y == 6)
                {
                    cell.Figure = new Pawn("pawn", Side.White);
                }

                if (y == 0 && (x == 0 || x == xSize - 1))
                {
                    cell.Figure = new Tower("tower", Side.Black);
                }

                if (y == ySize - 1 && (x == 0 || x == xSize - 1))
                {
                    cell.Figure = new Tower("tower", Side.White);
                }

                if (y == 0 && (x == 2 || x == xSize - 3))
                {
                    cell.Figure = new Bishop("bishop", Side.Black);
                }
                if (y == ySize - 1 && (x == 2 || x == xSize - 3))
                {
                    cell.Figure = new Bishop("bishop", Side.White);
                }
                if (y == 0 && x == xSize - 5)
                {
                    cell.Figure = new Queen("queen", Side.Black);
                }

                if (x == 3 && y == ySize - 1)
                {
                    cell.Figure = new Queen("queen", Side.White);
                }

                if (y == 0 && (x == 1 || x == xSize - 2))
                {
                    cell.Figure = new Knight("knight", Side.Black);
                }

                if (y == ySize - 1 && (x == 1 || x == xSize - 2))
                {
                    cell.Figure = new Knight("knight", Side.White);
                }

                if (y == 0 && (x == 4 || x == xSize - 4))
                {
                    cell.Figure = new King("king", Side.Black);
                }

                if (y == ySize - 1 && (x == 4 || x == xSize - 4))
                {
                    cell.Figure = new King("king", Side.White);
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