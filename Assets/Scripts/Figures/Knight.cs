using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Figure
{

    public Knight(string name):base(name)
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
        result[0].Add((2,1));
        result[1].Add((2,-1));
        result[2].Add((-2,-1));
        result[3].Add((-2,1));
        result[4].Add((1,2));
        result[5].Add((1,-2));
        result[6].Add((-1,2));
        result[7].Add((-1,-2));
        return result;
            
    }
}
