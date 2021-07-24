using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTurn : Turn
{
    private readonly Action<(int, int), (int, int)> _moveFigure;

    public MovingTurn((int, int) subjectCoords, (int, int) objectCoords, Figure subjectFigure, Figure objectFigure,
        Action<(int, int), (int, int)> moveFigure) :
        base(subjectCoords, objectCoords, subjectFigure, objectFigure)
    {
        _moveFigure = moveFigure;
    }

    public override void DoTurn()
    {
        base.DoTurn();
        _moveFigure(SubjectCoords, ObjectCoords);
        SubjectFigure.OnMove?.Invoke();
    }
}