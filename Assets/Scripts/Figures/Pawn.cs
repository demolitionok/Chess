using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Pawn : Figure
{
    public bool IsMoved = false;

    public Pawn(string name, Side side):base(name, side)
    {
        OnMove += () => { IsMoved = true;};
        Sprite = Side == Side.White ? Resources.Load<Sprite>("wpawn") : Resources.Load<Sprite>("bpawn");
    }

    public override List<CoordsList> GetRelativeAttacks((int, int) size)
    {
        return new List<CoordsList>{new CoordsList{(1, 1)}, new CoordsList{(1, -1)}};
    }
    public override List<CoordsList> GetRelativeMoves((int, int) size)
    {
        return new List<CoordsList>{new CoordsList{(1, 0)}};
    }
}
