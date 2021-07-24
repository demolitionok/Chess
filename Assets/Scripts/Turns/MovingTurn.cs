using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingTurn : Turn
{
    protected readonly Action<(int, int), (int, int)> MoveFigure;

    protected MovingTurn((int, int) subjectCoords, (int, int) objectCoords, Figure subjectFigure, Figure objectFigure,
        Action<(int, int), (int, int)> moveFigure) :
        base(subjectCoords, objectCoords, subjectFigure, objectFigure)
    {
        MoveFigure = moveFigure;
    }

    public override void DoTurn()
    {
        base.DoTurn();
        SubjectFigure.OnMove?.Invoke();
    }
}