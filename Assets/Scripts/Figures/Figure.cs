using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnMove();
public abstract class Figure
{
    public string Name;
    public OnMove OnMove;
    public abstract List<List<(int, int)>> GetRelativeMoves((int, int) size);//!should be written in (x, y) format
    public abstract List<List<(int, int)>> GetRelativeAttacks((int, int) size);
    public Figure(string name)
    {
        Name = name;
    }


}
