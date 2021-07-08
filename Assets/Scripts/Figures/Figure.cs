using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure
{
    //TODO bool CanMove(Cell d)
    //TODO List<cell> movements
    public List<(int, int)> GetMovements()//!should be written in (x, y) format
    {
        return new List<(int, int)>{(25, 25)};
    }

    public bool CanMove((int, int) coords)
    {
        if (GameController.GetCellByCoords(coords).State == null)
            return true;
        return false;
    }

    public List<(int, int)> GetPossibleMovements()
    {
        var result = new List<(int, int)>();
        foreach (var movement in GetMovements())
        {
            if (CanMove(movement))
                result.Add(movement);
        }

        return result;
    }
}
