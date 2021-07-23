using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CoordsList : IEnumerable<(int, int)>
{
    private readonly List<(int, int)> _coords = new List<(int, int)>();

    public IEnumerator<(int, int)> GetEnumerator() => _coords.GetEnumerator();


    public void Add((int, int) coords) => _coords.Add(coords);
    public int Count => _coords.Count;
    public bool Contains((int, int) coords) => _coords.Contains(coords);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    

    public (int, int) this[int index]
    {
        get => _coords[index];
        set => _coords[index] = value;
    }
}