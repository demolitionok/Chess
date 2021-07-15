using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public delegate void OnTrySelectCoords((int, int) coords);
public delegate void OnSelected();

public class GameController : MonoBehaviour
{
    public Board Board;
    public (int, int)? selectedCoords;
    public OnTrySelectCoords OnTrySelectCoords;
    public OnSelected OnSelected;
    public Side currentSide = Side.White;
    

    public void MoveFigure((int, int) from, (int, int) to)
    {
        Board.GetCellByCoords(to).Figure = Board.GetCellByCoords(from).Figure;
        Board.GetCellByCoords(from).Figure = null;
    }
    private bool CanMove((int, int) coords)
    {
        try
        {
            var fig = Board.GetCellByCoords(coords).Figure;
            return fig == null;
        }
        catch (IndexOutOfRangeException e)
        {
        }
        return false;
    }
    private bool CanAttack((int, int) coords)
    {
        try
        {
            var fig = Board.GetCellByCoords(coords).Figure;
            if(fig != null)
                return fig.Side != currentSide;
        }
        catch (IndexOutOfRangeException e)
        {
        }
        return false;
    }
    
    private List<(int, int)> GetPossibleAttacks((int, int) selectedFigureCoords)
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
    
    private List<List<(int, int)>> ConvertRelativeDirectionsToGlobal(List<List<(int, int)>> relativeDirections, (int, int) pivotCoords)
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

    private (int,int) ReverseY((int,int) YXcoord)
    {
        return (-YXcoord.Item1, YXcoord.Item2);
    }

    private List<List<(int, int)>> GetFigureGlobalMovementCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = Board.GetCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeMoves((Board.ySize, Board.xSize));
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
    
    private List<List<(int, int)>> GetFigureGlobalAttackCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = Board.GetCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeAttacks((Board.ySize, Board.xSize));
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

    private List<(int, int)> GetPossibleMoves((int, int) selectedFigureCoords)
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

    public List<(int, int)> GetActualMoves((int, int) selectedFigureCoords)
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

    public List<(int, int)> GetActualAttacks((int, int) selectedFigureCoords)
    {
        var result = new List<(int, int)>();
        foreach (var move in GetPossibleAttacks(selectedFigureCoords))
        {
            var currentCell = Board.GetCellByCoords(selectedFigureCoords);
            var destinationCell = Board.GetCellByCoords(move);
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


    public void SetCellInfoText((int, int) coords)
    {
        var cell = Board.GetCellByCoords(coords);
        var textTranform = cell.gameObject.transform.GetChild(0);
        var textStr = $" {cell.Figure.Side}";
        textStr += $"\n {coords.Item2}, {coords.Item1}";
        var figureName = cell.Figure == null ? "noFigure" : cell.Figure.Name;
        textStr += $"\n {figureName}";
        textTranform.gameObject.GetComponent<Text>().text = textStr;
    }


    public bool IsCheck()
    {
        currentSide = currentSide == Side.White ? Side.Black : Side.White;
        for (int y = 0; y < Board.ySize; y++)
        {
            for (int x = 0; x < Board.xSize; x++)
            {
                var curCell = Board.GetCellByCoords((y, x));
                
                if (curCell.Figure != null)
                {
                    if (curCell.Figure.Side == currentSide)
                    {
                        var possibleAttacks = GetPossibleAttacks((y, x));
                        foreach (var possibleAttack in possibleAttacks)
                        {
                            if (Board.GetCellByCoords(possibleAttack).Figure.GetType() == typeof(King))
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

    public bool IsMate()
    {
        for (int y = 0; y < Board.ySize; y++)
        {
            for (int x = 0; x < Board.xSize; x++)
            {
                var curCell = Board.GetCellByCoords((y, x));

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

    public void TrySelectCoords((int, int) coords)//!should be written in (y, x) format
    {
        var nextCell = Board.GetCellByCoords(coords);
        
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
            var currentCell = Board.GetCellByCoords(selectedCoords.Value);
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
        OnSelected.Invoke();
    }

    private void Start()
    {
        OnTrySelectCoords += TrySelectCoords;
    }
}
