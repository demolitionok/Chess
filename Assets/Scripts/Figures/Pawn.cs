using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Figure
{
    public bool IsMoved;
    public List<(int, int)> GetMovements(Side side)
    {
        if (side == Side.White)
        {
            return new List<(int, int)>{(0, 1)};
        }
        return new List<(int, int)>{(0, -1)};
    }
}
