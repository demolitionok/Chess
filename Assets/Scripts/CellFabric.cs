using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CellFabric
{
    public static List<Cell> Cells = new List<Cell>();

    public static void CreateCell(GameObject CellPrefab, (int, int) Coords)
    {
        Cell cell = new Cell
        {
            CellPrefab = CellPrefab,
            Coords = Coords
        };
        Cells.Add(cell);
    }
}
