using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    CancelSelection,
    SelectNewFigure,
    AttackFigure,
    MoveFigure,
    Castling
}

public delegate void ActionSend((int, int) coords);
public delegate void ActionRequest((int, int) coords, PlayerAction playerAction);

public class InputController
{
    private readonly Func<Side> getCurrentSide;
    private readonly Func<(int, int)?> getSelectedCoords;
    private readonly Func<(int, int), Cell> getCellByCoords;
    public ActionRequest ActionRequest;

    public void RequestAction((int, int) coords)
    {
        var nextCell = getCellByCoords(coords);
        var selectedCoords = getSelectedCoords.Invoke();

        var currentSide = getCurrentSide.Invoke();
        
        if (selectedCoords == null)
        {
            if (nextCell.Figure != null)
            {
                if (currentSide == nextCell.Figure.Side)
                {
                    ActionRequest(coords, PlayerAction.SelectNewFigure);
                }
            }
        }
        else
        {
            var selectedCell = getCellByCoords(selectedCoords.Value);
            if (selectedCoords == coords)
            {
                ActionRequest(coords, PlayerAction.CancelSelection);
            }

            else if (nextCell.Figure != null)
            {
                if (currentSide == nextCell.Figure.Side)
                {
                    if (nextCell.Figure is Tower tower && selectedCell.Figure is King king)
                    {
                        if(!king.IsMoved && !tower.IsMoved)
                            ActionRequest(coords, PlayerAction.Castling);
                    }
                    else
                    {
                        ActionRequest(coords, PlayerAction.SelectNewFigure);
                    }
                }
                else
                {
                    ActionRequest(coords, PlayerAction.AttackFigure);
                }
            }
            else
            {
                ActionRequest(coords, PlayerAction.MoveFigure);
            }
        }
    }

    public InputController(Func<Side> getCurrentSide, Func<(int, int)?> getSelectedCoords,
        Func<(int, int), Cell> getCellByCoords)
    {
        this.getCurrentSide = getCurrentSide;
        this.getSelectedCoords = getSelectedCoords;
        this.getCellByCoords = getCellByCoords;
    }
}