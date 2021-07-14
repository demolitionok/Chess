using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public delegate void OnTrySelectCoords((int, int) coords);
public static class GameController
{
    public static GameObject[,] CellsGameObjects = new GameObject[8,8];// перенести в боард
    public static (int, int)? selectedCoords;
    public static OnTrySelectCoords OnTrySelectCoords;
    public static Side currentSide = Side.White;

    public static Cell GetCellByCoords((int,int) coords)//!coords should be written in (y, x) format
    {
        return CellsGameObjects[coords.Item1, coords.Item2].GetComponent<Cell>();
        
    }

    public static void RenderAttacks(List<(int, int)> possibleAttacks)
    {
        foreach (var attack in possibleAttacks)
        {
            var curCell = GetCellByCoords(attack);
            var colors = curCell.gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.red;
            colors.selectedColor = Color.red;
            curCell.gameObject.GetComponent<Button>().colors = colors;
        }
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
        GetCellByCoords(to).Figure = GetCellByCoords(from).Figure;
        GetCellByCoords(from).Figure = null;
    }
    
    public static bool CanMove((int, int) coords)
    {
        try
        {
            var fig = GetCellByCoords(coords).Figure;
            return fig == null;
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
            var fig = GetCellByCoords(coords).Figure;
            if(fig != null)
                return fig.Side != currentSide;
        }
        catch (IndexOutOfRangeException e)
        {
        }
        return false;
    }
    
    public static List<(int, int)> GetPossibleAttacks((int, int) selectedFigureCoords)
    {
        var result = new List<(int, int)>();
        foreach (var globalDirection in GetFigureGlobalAttackCoords(selectedFigureCoords))
        {
            foreach (var globalCoords in globalDirection)
            {
                 if (CanAttack(globalCoords))
                 {
                    result.Add(globalCoords);
                    break;
                 } 
                 if (!CanMove(globalCoords))
                 {
                    break;
                 }
            }
        }
        
        return result;
    }
    
    public static List<List<(int, int)>> ConvertRelativeDirectionsToGlobal(List<List<(int, int)>> relativeDirections, (int, int) pivotCoords)
    {
        var result = new List<List<(int, int)>>();
        foreach (var direction in relativeDirections)
        {
            var globalDirection = new List<(int,int)>();
            foreach (var move in direction) 
            {
                globalDirection.Add((move.Item1 + pivotCoords.Item1, move.Item2 + pivotCoords.Item2));
            }
            result.Add(globalDirection);
        }

        return result;
    }

    public static (int,int) ReverseY((int,int) YXcoord)
    {
        return (-YXcoord.Item1, YXcoord.Item2);
    }

    public static List<List<(int, int)>> GetFigureGlobalMovementCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = GetCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeMoves((CellsGameObjects.GetLength(0), CellsGameObjects.GetLength(1)));
        if (selectedFigure is Pawn pawnFig)
        {

            if (!pawnFig.IsMoved)
            {
                relativeDirections[0].Add((2, 0));
            }

            if (selectedCell.Figure.Side == Side.White)
            {
                for (int y = 0; y < relativeDirections.Count; y++)
                {
                    for (int x = 0; x < relativeDirections[y].Count; x++)
                    {
                        relativeDirections[y][x] = ReverseY(relativeDirections[y][x]);
                    }
                }
            }
        }
        return ConvertRelativeDirectionsToGlobal(relativeDirections, selectedFigureCoords);
    }
    
    public static List<List<(int, int)>> GetFigureGlobalAttackCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = GetCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeAttacks((CellsGameObjects.GetLength(0), CellsGameObjects.GetLength(1)));
        if (selectedFigure.GetType() == typeof(Pawn) && selectedCell.Figure.Side == Side.White)
        {
            for(int y = 0; y < relativeDirections.Count; y++)
            {
                for(int x = 0; x < relativeDirections[y].Count; x++)
                {
                    relativeDirections[y][x] = ReverseY(relativeDirections[y][x]);
                }
            }
        }
        return ConvertRelativeDirectionsToGlobal(relativeDirections, selectedFigureCoords);
    }

    public static List<(int, int)> GetPossibleMoves((int, int) selectedFigureCoords)
    {
        var result = new List<(int, int)>();
        foreach (var globalDirection in GetFigureGlobalMovementCoords(selectedFigureCoords))
        {
            foreach (var globalCoords in globalDirection)
            {
                if (CanMove(globalCoords))
                {
                    //virtual movement
                    result.Add(globalCoords);
                
                }
                else
                {
                    break;
                }
            }
        }
        
        return result;
    }

    public static List<(int, int)> GetActualMoves((int, int) selectedFigureCoords)
    {
        var result = new List<(int, int)>();
        foreach (var move in GetPossibleMoves(selectedFigureCoords))
        {
            MoveFigure(selectedFigureCoords, move);
            if (!IsCheck())
            {
                result.Add(move);
            }
            MoveFigure(move, selectedFigureCoords);
        }
        return result;
    }

    public static List<(int, int)> GetActualAttacks((int, int) selectedFigureCoords)
    {
        var result = new List<(int, int)>();
        foreach (var move in GetPossibleAttacks(selectedFigureCoords))
        {
            var currentCell = GetCellByCoords(selectedFigureCoords);
            var destinationCell = GetCellByCoords(move);
            var destinationCellFigure = destinationCell.Figure;
            MoveFigure(selectedFigureCoords, move);
            if (!IsCheck())
            {
                result.Add(move);
            }
            MoveFigure(move, selectedFigureCoords);
            destinationCell.Figure = destinationCellFigure;
        }
        return result;
    }

    public static void ColorButton(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    public static void SetCellInfoText((int, int) coords)
    {
        var cell = GetCellByCoords(coords);
        var textTranform = cell.gameObject.transform.GetChild(0);
        var textStr = $" {cell.Figure.Side}";
        textStr += $"\n {coords.Item2}, {coords.Item1}";
        var figureName = cell.Figure == null ? "noFigure" : cell.Figure.Name;
        textStr += $"\n {figureName}";
        textTranform.gameObject.GetComponent<Text>().text = textStr;
    }

    public static void SetFigureImageByCoords((int, int) coords, Sprite sprite)
    {
        GetCellByCoords(coords).gameObject.GetComponent<Image>().sprite = sprite;
    }
    public static void UpdateBoard()
    {
        for (int y = 0; y < CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < CellsGameObjects.GetLength(1); x++)
            {
                var curCell = GetCellByCoords((y, x));
                //SetCellInfoText((y,x));
                var curCellButton = curCell.gameObject.GetComponent<Button>();
                ColorButton(curCellButton, selectedCoords == (y, x) ? Color.green : Color.white);
                if (curCell.Figure != null) 
                {
                    SetFigureImageByCoords((y, x), curCell.Figure.Sprite);
                }
                else
                {
                    SetFigureImageByCoords((y, x), Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero));
                }
            }
        }

        try//throws exception only before first figure selection because selectedCoords = null
        {
            RenderMoves(GetActualMoves(selectedCoords.Value));
            RenderAttacks(GetActualAttacks(selectedCoords.Value));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static bool IsCheck()
    {
        currentSide = currentSide == Side.White ? Side.Black : Side.White;
        for (int y = 0; y < CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < CellsGameObjects.GetLength(1); x++)
            {
                var curCell = GetCellByCoords((y, x));
                
                if (curCell.Figure != null)
                {
                    if (curCell.Figure.Side == currentSide)
                    {
                        var possibleAttacks = GetPossibleAttacks((y, x));
                        foreach (var possibleAttack in possibleAttacks)
                        {
                            if (GetCellByCoords(possibleAttack).Figure.GetType() == typeof(King))
                            {
                                currentSide = currentSide == Side.White ? Side.Black : Side.White;
                                Debug.Log($"Checked");
                                return true;
                            }
                        }
                    }
                }
            }
        }
        currentSide = currentSide == Side.White ? Side.Black : Side.White;
        return false;
    }

    public static bool IsMate()
    {
        for (int y = 0; y < CellsGameObjects.GetLength(0); y++)
        {
            for (int x = 0; x < CellsGameObjects.GetLength(1); x++)
            {
                var curCell = GetCellByCoords((y, x));

                if (curCell.Figure != null)
                {
                    if (curCell.Figure.Side == currentSide)
                    {
                        if (GetActualAttacks((y, x)).Count != 0 || GetActualMoves((y, x)).Count != 0)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public static void TrySelectCoords((int, int) coords)//!should be written in (y, x) format
    {
        var nextCell = GetCellByCoords(coords);
        
        if (selectedCoords == null)
        {
            if (nextCell.Figure != null)
            {
                if (currentSide == nextCell.Figure.Side)
                {
                    selectedCoords = coords;
                }
            }
        }
        else
        {
            var currentCell = GetCellByCoords(selectedCoords.Value);
            var currentFigure = currentCell.Figure;
            if (selectedCoords == coords)
            {
                selectedCoords = null;
            }
            else if (nextCell.Figure != null)
            {
                if (currentSide == nextCell.Figure.Side)
                {
                    selectedCoords = coords;
                }
                else
                {
                    var possibleAttacks = GetActualAttacks(selectedCoords.Value);
                    if (possibleAttacks.Contains(coords))
                    {
                        MoveFigure(selectedCoords.Value, coords);
                        currentFigure.OnMove?.Invoke();
                        selectedCoords = null;
                        currentSide = currentSide == Side.White ? Side.Black : Side.White;
                    }
                }
            }
            else
            {
                var actualMoves = GetActualMoves(selectedCoords.Value);
                if (actualMoves.Contains(coords))
                {
                    MoveFigure(selectedCoords.Value, coords);
                    currentFigure.OnMove?.Invoke();
                    selectedCoords = null;
                    currentSide = currentSide == Side.White ? Side.Black : Side.White;
                }
            }

            if (IsMate())
            {
                string winnerSide = currentSide == Side.White ? "black" : "white";
                Debug.Log($"Winner: {winnerSide}");
            }
        }
        UpdateBoard();
    }
}
