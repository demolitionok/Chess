using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public delegate void OnSelection();

public delegate void OnPlayerTurn();

public class GameController
{
    public OnSelection OnSelection;
    public OnPlayerTurn OnPlayerTurn;
    private (int, int)? selectedCoords;
    private Side currentSide = Side.White;
    private readonly Func<(int, int), Cell> getCellByCoords;
    private readonly int xSize;
    private readonly int ySize;

    public (int, int)? GetSelectedCoords() => selectedCoords;
    public Side GetCurrentSide() => currentSide;

    private void PlaceFigureTo((int, int) from, (int, int) to)
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
        var selectedFigure = getCellByCoords(selectedFigureCoords).Figure;
        if (selectedFigure is King king)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    var destCoords = (y, x);
                    var curCell = getCellByCoords(destCoords);

                    if (curCell.Figure is Tower tower)
                    {
                        if (curCell.Figure.Side == currentSide && !tower.IsMoved && !king.IsMoved)
                        {
                            var isPathClear = IsPathClear(selectedFigureCoords, destCoords);
                            var isPathWithoutCheck = IsPathWithoutChecks(selectedFigureCoords, destCoords);
                            if (isPathClear && isPathWithoutCheck)
                            {
                                result.Add(destCoords);
                            }
                        }
                    }
                }
            }
        }

        foreach (var move in GetPossibleMoves(selectedFigureCoords))
        {
            if (!IsCheckAt(selectedFigureCoords, move))
            {
                result.Add(move);
            }
        }

        return result;
    }

    private bool IsCheckAt((int, int) selectedFigureCoords, (int, int) move)
    {
        var destinationCell = getCellByCoords(move);
        var destinationCellFigure = destinationCell.Figure;

        PlaceFigureTo(selectedFigureCoords, move);

        var result = IsCheck();

        PlaceFigureTo(move, selectedFigureCoords);

        destinationCell.Figure = destinationCellFigure;

        return result;
    }

    public CoordsList GetActualAttacks((int, int) selectedFigureCoords)
    {
        var result = new CoordsList();
        foreach (var move in GetPossibleAttacks(selectedFigureCoords))
        {
            if (!IsCheckAt(selectedFigureCoords, move))
            {
                result.Add(move);
            }
        }

        return result;
    }

    private CoordsList Path((int, int) from, (int, int) to)
    {
        var result = new CoordsList();
        var bigger = from.Item2 < to.Item2;
        var start = bigger ? from : to;
        var end = bigger ? to : from;
        for (int y = start.Item1; y <= end.Item1; y++)
        {
            for (int x = start.Item2 + 1; x < end.Item2; x++)
            {
                result.Add((y, x));
            }
        }

        return result;
    }

    private bool IsPathClear((int, int) from, (int, int) to)
    {
        var path = Path(from, to);
        foreach (var coords in path)
        {
            if (!CanMove(coords))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsPathWithoutChecks((int, int) from, (int, int) to)
    {
        var path = Path(from, to);
        foreach (var coords in path)
        {
            if (IsCheckAt(from, coords))
            {
                return false;
            }
        }

        return true;
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

    //TODO: turn and OnTurn(
    public void ProcessAction((int, int) coords, PlayerAction playerAction) //!should be written in (y, x) format
    {
        switch (playerAction)
        {
            case PlayerAction.CancelSelection:
                selectedCoords = null;
                break;
            case PlayerAction.SelectNewFigure:
                selectedCoords = coords;
                break;
            case PlayerAction.AttackFigure:
            {
                var currentCell = getCellByCoords(selectedCoords.Value);
                var nextCell = getCellByCoords(coords);

                var nextFigure = nextCell.Figure;
                var currentFigure = currentCell.Figure;

                var possibleAttacks = GetActualAttacks(selectedCoords.Value);
                if (possibleAttacks.Contains(coords))
                {
                    var attackTurn = new MoveTurn(selectedCoords.Value, coords, currentFigure, nextFigure,
                        PlaceFigureTo);

                    attackTurn.DoTurn();
                    OnPlayerTurn.Invoke();
                }

                break;
            }
            case PlayerAction.MoveFigure:
            {
                var currentCell = getCellByCoords(selectedCoords.Value);
                var nextCell = getCellByCoords(coords);

                var currentFigure = currentCell.Figure;
                var nextFigure = nextCell.Figure;

                var actualMoves = GetActualMoves(selectedCoords.Value);
                if (actualMoves.Contains(coords))
                {
                    var moveTurn = new MoveTurn(selectedCoords.Value, coords, currentFigure, nextFigure,
                        PlaceFigureTo);

                    moveTurn.DoTurn();
                    OnPlayerTurn.Invoke();
                }

                break;
            }
            case PlayerAction.Castling:
            {
                var currentCell = getCellByCoords(selectedCoords.Value);
                var nextCell = getCellByCoords(coords);

                var currentFigure = currentCell.Figure;
                var nextFigure = nextCell.Figure;

                var actualMoves = GetActualMoves(selectedCoords.Value);
                if (actualMoves.Contains(coords))
                {
                    var castlingTurn = new CastlingTurn(selectedCoords.Value, coords, currentFigure, nextFigure,
                        PlaceFigureTo);

                    castlingTurn.DoTurn();
                    OnPlayerTurn.Invoke();
                }

                break;
            }
            default:
                throw new NotImplementedException();
        }

        OnSelection.Invoke();
    }

    public GameController(Func<(int, int), Cell> getCellByCoords, int ySize, int xSize)
    {
        this.getCellByCoords = getCellByCoords;
        this.ySize = ySize;
        this.xSize = xSize;
        OnPlayerTurn += () =>
        {
            selectedCoords = null;
            currentSide = currentSide == Side.White ? Side.Black : Side.White;
        };
    }
}