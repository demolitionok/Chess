using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTrySelectCoords((int, int) coords);
public static class GameController
{
    public static GameObject[,] CellsGameObjects = new GameObject[8,8];
    public static (int, int)? selectedCoords;
    public static OnTrySelectCoords OnTrySelectCoords;
    public static Side currentSide = Side.Black;

    public static Cell GetCellByCoords((int,int) coords)//!coords should be written in (y, x) format
    {
        return CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
    }

    public static void MoveFigure((int, int) from, (int, int) to)
    {
        var s = GetCellByCoords(from).State;
        var f = GetCellByCoords(from).Figure;
        GetCellByCoords(from).Figure = null;
        GetCellByCoords(from).State = null;
        GetCellByCoords(to).Figure = f;
        GetCellByCoords(to).State = s;
    }
    
    public static bool CanMove((int, int) coords)
    {
        if (GetCellByCoords(coords).State == null)
            return true;
        return false;
    }
    
    public static List<(int, int)> GetPossibleMoves((int, int) selectedFigureCoords)
    {
        var selectedFigure = GetCellByCoords(selectedFigureCoords).Figure;
        var result = new List<(int, int)>();
        foreach (var movement in selectedFigure.GetRelativeMoves())
        {
            if (CanMove((movement.Item2 + selectedFigureCoords.Item2, movement.Item1 + selectedFigureCoords.Item1)))
                result.Add((movement.Item2 + selectedFigureCoords.Item2, movement.Item1 + selectedFigureCoords.Item1));
        }

        return result;
    }
}
