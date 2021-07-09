using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Figure
{
    public Bishop(string name):base(name)
    {
    }
    
    public override List<List<(int, int)>> GetRelativeAttacks((int, int) size)
    {
        return GetRelativeMoves(size);
    }
    public override List<List<(int, int)>> GetRelativeMoves((int, int) size)
    {
        var result = new List<List<(int, int)>>();
        for (int i = 0; i < 4; i++)
        {
            var direction = new List<(int, int)>();
            result.Add(direction);
        }

        for (int k = 1; k < size.Item1; k++)
        {
            result[0].Add((k,k));
            result[2].Add((k,-k));
        }
        for (int k = 1; k < size.Item2; k++)
        {
            result[1].Add((-k,-k));
            result[3].Add((-k,k));
        }
        return result;
    }
}
