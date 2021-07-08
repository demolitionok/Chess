using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    Black,
    White
}

public class Cell : MonoBehaviour
{
    public Figure CellFigure;
    public Side? State;
}
