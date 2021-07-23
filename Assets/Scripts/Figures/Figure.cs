using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnMove();
public abstract class Figure
{
    [SerializeField] private string _name;
    [SerializeField] public string Name => _name;
    public Sprite Sprite;
    public Side Side;
    public OnMove OnMove;
    public abstract List<CoordsList> GetRelativeMoves((int, int) size);//!should be written in (x, y) format
    public abstract List<CoordsList> GetRelativeAttacks((int, int) size);
    protected Figure(string name, Side side)
    {
        _name = name;
        Side = side;
    }


}
