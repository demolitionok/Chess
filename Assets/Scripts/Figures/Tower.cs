using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Figure
{
    public Tower(string name, Side side):base(name, side)
    {
        Sprite = side == Side.White ? Resources.Load<Sprite>("wtower") : Resources.Load<Sprite>("btower");
    }
    
    public override List<CoordsList> GetRelativeAttacks((int, int) size)
    {
        return GetRelativeMoves(size);
    }
    public override List<CoordsList> GetRelativeMoves((int, int) size)
    {
        var result = new List<CoordsList>();
        for (int i = 0; i < 4; i++)
        {
            var direction = new CoordsList();
            result.Add(direction);
        }

        for (int k = 1; k < size.Item1; k++)
        {
            result[0].Add((0,k));
            result[2].Add((0,-k));
        }
        for (int k = 1; k < size.Item2; k++)
        {
            result[1].Add((k,0));
            result[3].Add((-k,0));
        }
        return result;
    }
}
