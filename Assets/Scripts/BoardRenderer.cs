using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardRenderer
{
    private readonly Func<(int, int), Cell> GetCellByCoords;

    public void ColorButton(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    private void ColorCells(CoordsList cellsCoordsToColor, Color color)
    {
        foreach (var coords in cellsCoordsToColor)
        {
            var curCell = GetCellByCoords(coords);
            var button = curCell.gameObject.GetComponent<Button>();
            ColorButton(button, color);
        }
    }

    public void RenderAttacks(CoordsList possibleAttacks) => ColorCells(possibleAttacks, Color.red);
    public void RenderMoves(CoordsList possibleMoves) => ColorCells(possibleMoves, Color.blue);

    public void SetFigureImageByCoords((int, int) coords, Sprite sprite)
    {
        GetCellByCoords(coords).gameObject.GetComponent<Image>().sprite = sprite;
    }

    public void SetCellInfoText((int, int) coords)
    {
        var cell = GetCellByCoords(coords);
        var textTranform = cell.gameObject.transform.GetChild(0);
        var textStr = $" {cell.Figure.Side}";
        textStr += $"\n {coords.Item2}, {coords.Item1}";
        var figureName = cell.Figure == null ? "noFigure" : cell.Figure.Name;
        textStr += $"\n {figureName}";
        textTranform.gameObject.GetComponent<Text>().text = textStr;
    }

    public BoardRenderer(Func<(int, int), Cell> getCellByCoords)
    {
        GetCellByCoords = getCellByCoords;
    }
}