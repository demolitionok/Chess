using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Figure
{
    public bool IsMoved;

    public Pawn(string name):base(name)
    {
    }

    public override List<(int, int)> GetRelativeMoves()
    {
        return new List<(int, int)>{(1, 0)};
    }
}
