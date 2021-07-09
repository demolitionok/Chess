using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Figure
{

    public King(string name):base(name)
    {
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
            result[0].Add((1,1));
            result[1].Add((0,1));
            result[2].Add((0,-1));
            result[3].Add((-1,0));
        }
        for (int k = 1; k < size.Item2; k++)
        {
            result[4].Add((-1,-1));
            result[5].Add((1,-1));
            result[6].Add((-1,1));
            result[7].Add((1, 0));
        }
        return result;
            
    }
}
