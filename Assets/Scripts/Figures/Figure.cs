using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnMove();
public abstract class Figure
{
    public string Name;
    public Sprite Sprite;
    public OnMove OnMove;
    public abstract List<List<(int, int)>> GetRelativeMoves((int, int) size);//!should be written in (x, y) format
    public abstract List<List<(int, int)>> GetRelativeAttacks((int, int) size);
    public Figure(string name, Sprite sprite)
    {
        Name = name;
        Sprite = sprite;
    }


}
