using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneTemplate;
using UnityEngine;

public static class CellFabric
{
    public static GameObject[,] Cells = new GameObject[8,8];

    public static void CreateCell(GameObject CellPrefab, (int, int) coords)
    {
        Cells[coords.Item1, coords.Item2] = CellPrefab; //x*i;y*i
    }
}
