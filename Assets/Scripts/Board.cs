using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject CellPrefab; 
    private void GenerateBoard()
    {
        for (int y = 0; y < CellFabric.Cells.GetLength(0); y++)
        {
            for (int x = 0; x < CellFabric.Cells.GetLength(1); x++)
            {
                CellFabric.CreateCell(CellPrefab, (x, y));
                GameObject cell = Instantiate(CellPrefab, new Vector3(x*32 + 16, -y*32 - 16), Quaternion.identity);
                cell.transform.SetParent(Canvas.transform, false);
                var text = cell.transform.GetChild(0);
                text.gameObject.GetComponent<Text>().text = $"{x}, {y}";
            }//x*i;y*i
        }
    }

    void Start()
    {
        GenerateBoard();
    }
    void Update()
    {
        
    }
}
