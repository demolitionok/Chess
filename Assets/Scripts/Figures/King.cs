using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Figure
{

    public King(string name, Side side):base(name, side)
    {
        Sprite = side == Side.White ? Resources.Load<Sprite>("wking") : Resources.Load<Sprite>("bking");
    }
    public override List<CoordsList> GetRelativeAttacks((int, int) size)
    {
        return GetRelativeMoves(size);
    }
    public override List<CoordsList> GetRelativeMoves((int, int) size)
    {
        var result = new List<CoordsList>();
        for (int i = 0; i < 8; i++)
        {
            var direction = new CoordsList();
            result.Add(direction);
        }
        result[0].Add((1,1));
        result[1].Add((0,1));
        result[2].Add((0,-1));
        result[3].Add((-1,0));
        result[4].Add((-1,-1));
        result[5].Add((1,-1));
        result[6].Add((-1,1));
        result[7].Add((1, 0));
        
        return result;
            
    }
}
