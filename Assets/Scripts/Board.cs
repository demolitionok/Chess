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

    public void UpdateBoard()
    {
        for (int y = 0; y < GameController.CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < GameController.CellsGameObjects.GetLength(1); x++)
            {
                Debug.Log("1111111");
                var curCell = GameController.CellsGameObjects[y, x].GetComponent<Cell>();
                
                if (GameController.selectedCoords == (y, x))
                {
                    Debug.Log("33333333");
                    var text = GameController.CellsGameObjects[y, x].transform.GetChild(0);
                    GameController.CellsGameObjects[y, x].transform.GetChild(0).gameObject.GetComponent<Text>().text = "MAYOT KRSER";
                }
            }
        }
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < GameController.CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < GameController.CellsGameObjects.GetLength(1); x++)
            {
                GameObject cellGameObject = Instantiate(CellPrefab, new Vector3(x*cellWidth + Offset, -y*cellHeight - Offset), Quaternion.identity);
                cellGameObject.transform.SetParent(Canvas.transform, false);
                
                CellFabric.CreateCell(cellGameObject, (y, x));
                
                cellGameObject = GameController.CellsGameObjects[y, x];
                var text = cellGameObject.transform.GetChild(0);//kostyl)
                text.gameObject.GetComponent<Text>().text = $"{x}, {y}";

                var cell = cellGameObject.GetComponent<Cell>();
                cell.State = null;
                //cell.x = x;
                //cell.y = y;
                if (x <= 7 && y <= 1)
                {
                    cell.State = Side.Black;
                }
                else if (x <= 7 && y >= 6)
                {
                    cell.State = Side.White;
                }

                text.gameObject.GetComponent<Text>().text += $" {cellGameObject.GetComponent<Cell>().State}";
            }
        }
    }
    public void TrySelectCoords((int, int) coords)
    {
        var cell = GameController.CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
        if (GameController.selectedCoords == null)
        {
            if (GameController.currentSide == cell.State)
            {
                GameController.selectedCoords = coords;
            }
        }
        else
        {
            if (GameController.selectedCoords == coords)
            {
                GameController.selectedCoords = null;
            }
            else if (GameController.currentSide == cell.State)
            {
                GameController.selectedCoords = coords;
            }
            else
            {
                //Move();
            }
        }
        UpdateBoard();
    }

    void Start()
    {
        GameController.OnTrySelectCoords += TrySelectCoords;
        cellWidth = CellPrefab.GetComponent<RectTransform>().rect.width;
        cellHeight = CellPrefab.GetComponent<RectTransform>().rect.height;
        GenerateBoard();
        
    }
    void Update()
    {
    }
}
