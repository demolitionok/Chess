using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    CancelSelection,
    SelectNewFigure,
    AttackFigure,
    MoveFigure
}

public delegate void OnActionSend((int, int) coords);
public delegate void OnActionRequest((int, int) coords, PlayerAction playerAction);

public class InputController
{
    private readonly Func<Side> getCurrentSide;
    private readonly Func<(int, int)?> getSelectedCoords;
    private readonly Func<(int, int), Cell> getCellByCoords;
    public OnActionRequest OnActionRequest;

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
                    OnActionRequest(coords, PlayerAction.SelectNewFigure);
                }
            }
        }
        else
        {
            if (selectedCoords == coords)
            {
                OnActionRequest(coords, PlayerAction.CancelSelection);
            }

            else if (nextCell.Figure != null)
            {
                OnActionRequest(coords, currentSide == nextCell.Figure.Side
                    ? PlayerAction.SelectNewFigure
                    : PlayerAction.AttackFigure);
            }
            else
            {
                OnActionRequest(coords, PlayerAction.MoveFigure);
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