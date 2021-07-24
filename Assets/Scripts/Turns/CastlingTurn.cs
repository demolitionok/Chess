using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingTurn : MovingTurn
{
    public CastlingTurn((int, int) subjectCoords, (int, int) objectCoords, Figure subjectFigure, Figure objectFigure,
        Action<(int, int), (int, int)> moveFigure) :
        base(subjectCoords, objectCoords, subjectFigure, objectFigure, moveFigure)
    {
    }

    public override void DoTurn()
    {
        base.DoTurn();
        var centerCoords = ((ObjectCoords.Item1 + SubjectCoords.Item1)/2, (ObjectCoords.Item2 + SubjectCoords.Item2)/2);
        var newKingCoords = centerCoords;
        var newRookCoords = (centerCoords.Item1, centerCoords.Item2 + 1);
        if (SubjectCoords.Item2 < ObjectCoords.Item2)
        {
            newKingCoords = (centerCoords.Item1, centerCoords.Item2 + 1);
            newRookCoords = centerCoords;
        }

        MoveFigure(SubjectCoords, newKingCoords);
        MoveFigure(ObjectCoords, newRookCoords);
    }
}