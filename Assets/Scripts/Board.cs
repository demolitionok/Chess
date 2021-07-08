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
        for (int y = 0; y < GameController.CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < GameController.CellsGameObjects.GetLength(1); x++)
            {
                GameObject cellGameObject = Instantiate(CellPrefab, new Vector3(x*cellWidth + Offset, -y*cellHeight - Offset), Quaternion.identity);
                cellGameObject.transform.SetParent(gameObject.transform, false);
                
                CellFabric.CreateCell(cellGameObject, (y, x));
                
                cellGameObject = GameController.CellsGameObjects[y, x];
                var text = cellGameObject.transform.GetChild(0);//kostyl)
                //text.gameObject.GetComponent<Text>().text = $"{x}, {y}";

                var cell = cellGameObject.GetComponent<Cell>();
                cell.State = null;
                if (y <= 1)
                {
                    cell.State = Side.Black;
                }
                else if (y >= 6)
                {
                    cell.State = Side.White;
                }

                if (y == 1 || y == 6)
                {
                    cell.Figure = new Pawn("Pawn");
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
        
    }
    void Update()
    {
    }
}
