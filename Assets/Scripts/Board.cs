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
        for (int y = 0; y < CellFabric.Cells.GetLength(0); y++)
        {
            for (int x = 0; x < CellFabric.Cells.GetLength(1); x++)
            {
                CellFabric.CreateCell(CellPrefab, (x, y));
                GameObject cell = Instantiate(CellPrefab, new Vector3(x*cellWidth + Offset, -y*cellHeight - Offset), Quaternion.identity);
                cell.transform.SetParent(Canvas.transform, false);
                var text = cell.transform.GetChild(0);//kostyl)
                text.gameObject.GetComponent<Text>().text = $"{x}, {y}";
                
                cell.GetComponent<Cell>().State = CellState.Empty;
                if (x <= 7 && y <= 1)
                {
                    cell.GetComponent<Cell>().State = CellState.Black;
                }
                else if (x <= 7 && y >= 6)
                {
                    cell.GetComponent<Cell>().State = CellState.White;
                }

                text.gameObject.GetComponent<Text>().text += $" {cell.GetComponent<Cell>().State}";
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
