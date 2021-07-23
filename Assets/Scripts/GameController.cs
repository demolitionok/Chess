using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public delegate void OnSelection();

public delegate void OnTrySelectCoords((int, int) coords);

public class GameController
{
    public (int, int)? selectedCoords;
    public OnTrySelectCoords OnTrySelectCoords;
    public OnSelection OnSelection;
    private Side currentSide = Side.White;
    private readonly Func<(int, int), Cell> getCellByCoords;
    private readonly int xSize;
    private readonly int ySize;

    private void MoveFigure((int, int) from, (int, int) to)
    {
        getCellByCoords(to).Figure = getCellByCoords(from).Figure;
        getCellByCoords(from).Figure = null;
    }

    private bool IsCoordsValid((int, int) coords)
    {
        return coords.Item1 < ySize && coords.Item1 >= 0 && coords.Item2 < xSize && coords.Item2 >= 0;
    }

    private bool CanMove((int, int) coords)
    {
        if (IsCoordsValid(coords))
        {
            var fig = getCellByCoords(coords).Figure;
            return fig == null;
        }

        return false;
    }

    private bool CanAttack((int, int) coords)
    {
        if (IsCoordsValid(coords))
        {
            var fig = getCellByCoords(coords).Figure;
            if (fig != null)
                return fig.Side != currentSide;
        }

        return false;
    }

    private CoordsList GetPossibleAttacks((int, int) selectedFigureCoords)
    {
        var result = new CoordsList();
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

    private List<CoordsList> ConvertRelativeDirectionsToGlobal(List<CoordsList> relativeDirections,
        (int, int) pivotCoords)
    {
        var result = new List<CoordsList>();
        foreach (var direction in relativeDirections)
        {
            var globalDirection = new CoordsList();
            foreach (var move in direction)
            {
                globalDirection.Add((move.Item1 + pivotCoords.Item1, move.Item2 + pivotCoords.Item2));
            }

            result.Add(globalDirection);
        }

        return result;
    }

    private (int, int) ReverseY((int, int) YXcoord)
    {
        return (-YXcoord.Item1, YXcoord.Item2);
    }

    private List<CoordsList> GetFigureGlobalMovementCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = getCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeMoves((ySize, xSize));
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

    private List<CoordsList> GetFigureGlobalAttackCoords((int, int) selectedFigureCoords)
    {
        var selectedCell = getCellByCoords(selectedFigureCoords);
        var selectedFigure = selectedCell.Figure;
        var relativeDirections = selectedFigure.GetRelativeAttacks((ySize, xSize));
        if (selectedFigure.GetType() == typeof(Pawn) && selectedCell.Figure.Side == Side.White)
        {
            for (int y = 0; y < relativeDirections.Count; y++)
            {
                for (int x = 0; x < relativeDirections[y].Count; x++)
                {
                    relativeDirections[y][x] = ReverseY(relativeDirections[y][x]);
                }
            }
        }

        return ConvertRelativeDirectionsToGlobal(relativeDirections, selectedFigureCoords);
    }
    private CoordsList GetPossibleMoves((int, int) selectedFigureCoords)
    {
        var result = new CoordsList();
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

    public CoordsList GetActualMoves((int, int) selectedFigureCoords)
    {
        var result = new CoordsList();
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

    public CoordsList GetActualAttacks((int, int) selectedFigureCoords)
    {
        var result = new CoordsList();
        foreach (var move in GetPossibleAttacks(selectedFigureCoords))
        {
            var currentCell = getCellByCoords(selectedFigureCoords);
            var destinationCell = getCellByCoords(move);
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


    private bool IsCheck()
    {
        currentSide = currentSide == Side.White ? Side.Black : Side.White;
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var curCell = getCellByCoords((y, x));

                if (curCell.Figure != null)
                {
                    if (curCell.Figure.Side == currentSide)
                    {
                        var possibleAttacks = GetPossibleAttacks((y, x));
                        foreach (var possibleAttack in possibleAttacks)
                        {
                            if (getCellByCoords(possibleAttack).Figure.GetType() == typeof(King))
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
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var curCell = getCellByCoords((y, x));

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

    //TODO: turn and OnTurn()
    public void TrySelectCoords((int, int) coords) //!should be written in (y, x) format
    {
        var nextCell = getCellByCoords(coords);

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
            var currentCell = getCellByCoords(selectedCoords.Value);
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

        OnSelection.Invoke();
    }

    public GameController(Func<(int, int), Cell> getCellByCoords, int ySize, int xSize)
    {
        this.getCellByCoords = getCellByCoords;
        this.ySize = ySize;
        this.xSize = xSize;
        OnTrySelectCoords += TrySelectCoords;
    }
}