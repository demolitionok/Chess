using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate Cell GetCellByCoords((int, int) _);

public static class BoardRenderer
{

    public static void ColorButton(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    private static void ColorCells(CoordsList cellsCoordsToColor, Color color, GetCellByCoords getCellByCoords)
    {
        foreach (var coords in cellsCoordsToColor)
        {
            var curCell = getCellByCoords(coords);
            var button = curCell.gameObject.GetComponent<Button>();
            ColorButton(button, color);
        }
    }

    public static void RenderAttacks(CoordsList possibleAttacks, GetCellByCoords getCellByCoords) => ColorCells(possibleAttacks, Color.red, getCellByCoords);
    public static void RenderMoves(CoordsList possibleMoves, GetCellByCoords getCellByCoords) => ColorCells(possibleMoves, Color.blue, getCellByCoords);

    public static void SetFigureImageByCoords((int, int) coords, Sprite sprite, GetCellByCoords getCellByCoords)
    {
        getCellByCoords(coords).gameObject.GetComponent<Image>().sprite = sprite;
    }

    public static void SetCellInfoText((int, int) coords, GetCellByCoords getCellByCoords)
    {
        var cell = getCellByCoords(coords);
        var textTranform = cell.gameObject.transform.GetChild(0);
        var textStr = $" {cell.Figure.Side}";
        textStr += $"\n {coords.Item2}, {coords.Item1}";
        var figureName = cell.Figure == null ? "noFigure" : cell.Figure.Name;
        textStr += $"\n {figureName}";
        textTranform.gameObject.GetComponent<Text>().text = textStr;
    }
}