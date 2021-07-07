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

    private void Render()
    {
        
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < GameController.CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < GameController.CellsGameObjects.GetLength(1); x++)
            {
                CellFabric.CreateCell(CellPrefab, (x, y));
                GameObject cellGameObject = Instantiate(CellPrefab, new Vector3(x*cellWidth + Offset, -y*cellHeight - Offset), Quaternion.identity);
                cellGameObject.transform.SetParent(Canvas.transform, false);
                var text = cellGameObject.transform.GetChild(0);//kostyl)
                text.gameObject.GetComponent<Text>().text = $"{x}, {y}";
                
                cellGameObject.GetComponent<Cell>().State = CellState.Empty;
                cellGameObject.GetComponent<Cell>().x = x;
                cellGameObject.GetComponent<Cell>().y = y;
                if (x <= 7 && y <= 1)
                {
                    cellGameObject.GetComponent<Cell>().State = CellState.Black;
                }
                else if (x <= 7 && y >= 6)
                {
                    cellGameObject.GetComponent<Cell>().State = CellState.White;
                }

                text.gameObject.GetComponent<Text>().text += $" {cellGameObject.GetComponent<Cell>().State}";
            }
        }
    }

    void Start()
    {
        cellWidth = CellPrefab.GetComponent<RectTransform>().rect.width;
        cellHeight = CellPrefab.GetComponent<RectTransform>().rect.height;
        GenerateBoard();
    }
    void Update()
    {
        
    }
}
