using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Figure
{
    public Queen(string name, Side side):base(name, side)
    {
        Sprite = side == Side.White ? Resources.Load<Sprite>("wqueen") : Resources.Load<Sprite>("bqueen");
    }
    
    public override List<List<(int, int)>> GetRelativeAttacks((int, int) size)
    {
        return GetRelativeMoves(size);
    }
    public override List<List<(int, int)>> GetRelativeMoves((int, int) size)
    {
        var result = new List<List<(int, int)>>();
        for (int i = 0; i < 8; i++)
        {
            var direction = new List<(int, int)>();
            result.Add(direction);
        }

        for (int k = 1; k < size.Item1; k++)
        {
            result[0].Add((0,k));
            result[1].Add((0,-k));
            result[2].Add((k,k));
            result[3].Add((k,-k));
        }
        for (int k = 1; k < size.Item2; k++)
        {
            result[4].Add((k,0));
            result[5].Add((-k,0));
            result[6].Add((-k,-k));
            result[7].Add((-k,k));
        }
        return result;
    }
}
