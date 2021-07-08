using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static void UpdateBoard()
    {
        for (int y = 0; y < GameController.CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < GameController.CellsGameObjects.GetLength(1); x++)
            {
                var curCell = GameController.GetCellByCoords((y, x));

                var t = GameController.CellsGameObjects[y, x].transform.GetChild(0);
                t.gameObject.GetComponent<Text>().text = $" {curCell.State}";
                t.gameObject.GetComponent<Text>().text += $"\n {x}, {y}";
                if(curCell.Figure != null) 
                    t.gameObject.GetComponent<Text>().text += $"\n {curCell.Figure.Name}";
                
                
                if (GameController.selectedCoords == (y, x))
                {
                    Debug.Log("correct click!");
                }
            }
        }
    }
    public static void TrySelectCoords((int, int) coords)//!should be written in (y, x) format
    {
        var cell = GameController.GetCellByCoords(coords);
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
                var possibleMovements = GameController.GetPossibleMoves(GameController.selectedCoords.Value);
                if (possibleMovements.Contains(coords))
                {
                    GameController.MoveFigure(GameController.selectedCoords.Value, coords);
                }
            }
        }
        UpdateBoard();
    }
}
