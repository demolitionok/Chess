using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTurn : MovingTurn
{
    public MoveTurn((int, int) subjectCoords, (int, int) objectCoords, Figure subjectFigure, Figure objectFigure,
        Action<(int, int), (int, int)> moveFigure) :
        base(subjectCoords, objectCoords, subjectFigure, objectFigure, moveFigure)
    {
    }

    public override void DoTurn()
    {
        base.DoTurn();
        MoveFigure(SubjectCoords, ObjectCoords);
    }
}