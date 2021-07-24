using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTurn();

public abstract class Turn
{
    protected readonly (int, int) SubjectCoords;
    protected readonly (int, int) ObjectCoords;
    protected readonly Figure SubjectFigure;
    protected readonly Figure ObjectFigure;

    protected Turn()
    {
        throw new NotImplementedException();
    }

    protected Turn((int, int) subjectCoords, (int, int) objectCoords, Figure subjectFigure, Figure objectFigure)
    {
        SubjectCoords = subjectCoords;
        ObjectCoords = objectCoords;
        SubjectFigure = subjectFigure;
        ObjectFigure = objectFigure;
    }

    public virtual void DoTurn(){}
}
