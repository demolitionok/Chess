using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnTrySelectCoords((int, int) coords);
public static class GameController
{
    public static GameObject[,] CellsGameObjects = new GameObject[8,8];
    public static (int, int)? selectedCoords;
    public static OnTrySelectCoords OnTrySelectCoords;
    public static Side currentSide = Side.White;

    public static Cell GetCellByCoords((int,int) coords)//!coords should be written in (y, x) format
    {
        return CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
        
    }

    public static void RenderMoves(List<(int, int)> possibleMoves)
    {
        foreach (var move in possibleMoves)
        {
            var curCell = GetCellByCoords(move);
            var colors = curCell.gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.blue;
            colors.selectedColor = Color.blue;
            curCell.gameObject.GetComponent<Button>().colors = colors;
        }
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
        try
        {
            return GetCellByCoords(coords).State == null;
        }
        catch (IndexOutOfRangeException e)
        {
        }
        return false;
    }
    public static bool CanAttack((int, int) coords)
    {
        try
        {
            return GetCellByCoords(coords).State != currentSide && GetCellByCoords(coords).State != null;
        }
        catch (IndexOutOfRangeException e)
        {
        }
        return false;
    }

    public static List<(int, int)> GetPossibleAttacks((int, int) selectedFigureCoords)
    {
        var selectedFigure = GetCellByCoords(selectedFigureCoords).Figure;
        var relativeAttacks = selectedFigure.GetRelativeAttacks((CellsGameObjects.GetLength(0), CellsGameObjects.GetLength(1)));
        if (selectedFigure.GetType() == typeof(Pawn) && GetCellByCoords(selectedFigureCoords).GetComponent<Cell>().State == Side.White)
        {
            if (GetCellByCoords(selectedFigureCoords).GetComponent<Cell>().State == Side.White)
            {
                relativeAttacks[0][0] = (-relativeAttacks[0][0].Item1, relativeAttacks[0][0].Item2);
                relativeAttacks[1][0] = (-relativeAttacks[0][0].Item1, relativeAttacks[0][0].Item2);
            }
        }

        var result = new List<(int, int)>();
        foreach (var direction in relativeAttacks)
        {
            foreach (var movement in direction)
            {
                var resultCoords = (movement.Item1 + selectedFigureCoords.Item1, movement.Item2 + selectedFigureCoords.Item2);
                if (CanAttack(resultCoords))
                {
                    result.Add(resultCoords);
                    break;
                }
            }
        }
        return result;
    }

    public static List<(int, int)> GetPossibleMoves((int, int) selectedFigureCoords)
    {
        var selectedFigure = GetCellByCoords(selectedFigureCoords).Figure;
        var relativeMoves = selectedFigure.GetRelativeMoves((CellsGameObjects.GetLength(0), CellsGameObjects.GetLength(1)));
        if (selectedFigure.GetType() == typeof(Pawn) && GetCellByCoords(selectedFigureCoords).GetComponent<Cell>().State == Side.White)
        {
            if (GetCellByCoords(selectedFigureCoords).GetComponent<Cell>().State == Side.White)
            {
                relativeMoves[0][0] = (-1, 0);
            }
        }

        var result = new List<(int, int)>();
        foreach (var direction in relativeMoves)
        {
            foreach (var movement in direction)
            {
                var resultCoords = (movement.Item1 + selectedFigureCoords.Item1, movement.Item2 + selectedFigureCoords.Item2);
                if (CanMove(resultCoords))
                {
                    result.Add(resultCoords);
                }
                else
                {
                    break;
                }
            }
        }

        return result;
    }

    public static void UpdateBoard()
    {
        for (int y = 0; y < CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < CellsGameObjects.GetLength(1); x++)
            {
                var curCell = GetCellByCoords((y, x));

                var t = CellsGameObjects[y, x].transform.GetChild(0);
                t.gameObject.GetComponent<Text>().text = $" {curCell.State}";
                t.gameObject.GetComponent<Text>().text += $"\n {x}, {y}";
                if(curCell.Figure != null) 
                    t.gameObject.GetComponent<Text>().text += $"\n {curCell.Figure.Name}";
                
                
                if (selectedCoords == (y, x))
                {
                    var colors = curCell.gameObject.GetComponent<Button>().colors;
                    colors.normalColor = Color.green;
                    colors.selectedColor = Color.green;
                    curCell.gameObject.GetComponent<Button>().colors = colors;
                }
                else
                {
                    var colors = curCell.gameObject.GetComponent<Button>().colors;
                    colors.normalColor = Color.white;
                    colors.selectedColor = Color.white;
                    curCell.gameObject.GetComponent<Button>().colors = colors;
                }
            }
        }

        try
        {
            RenderMoves(GetPossibleMoves(selectedCoords.Value));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public static void TrySelectCoords((int, int) coords)//!should be written in (y, x) format
    {
        var cell = GetCellByCoords(coords);
        if (selectedCoords == null)
        {
            if (currentSide == cell.State)
            {
                selectedCoords = coords;
            }
        }
        else
        {
            if (selectedCoords == coords)
            {
                selectedCoords = null;
            }
            else if (currentSide == cell.State)
            {
                selectedCoords = coords;
            }
            else if (cell.State != null)
            {
                var possibleAttacks = GetPossibleAttacks(selectedCoords.Value);
                if (possibleAttacks.Contains(coords))
                {
                    MoveFigure(selectedCoords.Value, coords);
                    selectedCoords = null;
                    currentSide = currentSide == Side.White ? Side.Black : Side.White;
                }
            }
            else
            {
                var possibleMovements = GetPossibleMoves(selectedCoords.Value);
                if (possibleMovements.Contains(coords))
                {
                    MoveFigure(selectedCoords.Value, coords);
                    selectedCoords = null;
                    currentSide = currentSide == Side.White ? Side.Black : Side.White;
                }
            }
        }
        UpdateBoard();
    }
}
