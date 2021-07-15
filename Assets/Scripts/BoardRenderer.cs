using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardRenderer : MonoBehaviour
{
    public Board Board;
    public GameController GameController;
    
    
    private void SetFigureImageByCoords((int, int) coords, Sprite sprite)
    {
        Board.GetCellByCoords(coords).gameObject.GetComponent<Image>().sprite = sprite;
    }
    
    private void ColorButton(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    private void ColorCells(List<(int,int)> cellsCoordsToColor, Color color)
    {
        foreach (var coords in cellsCoordsToColor)
        {
            var curCell = Board.GetCellByCoords(coords);
            var button = curCell.gameObject.GetComponent<Button>();
            ColorButton(button, color);
        }
    }
    private void RenderAttacks(List<(int, int)> possibleAttacks) => ColorCells(possibleAttacks, Color.red);
    private void RenderMoves(List<(int, int)> possibleMoves) => ColorCells(possibleMoves, Color.blue);
    
    public void UpdateBoard()
    {
        for (int y = 0; y < Board.ySize; y++)
        {
            for (int x = 0; x < Board.xSize; x++)
            {
                var curCell = Board.GetCellByCoords((y, x));
                //SetCellInfoText((y,x));
                var curCellButton = curCell.gameObject.GetComponent<Button>();
                ColorButton(curCellButton, GameController.selectedCoords == (y, x) ? Color.green : Color.white);
                if (curCell.Figure != null) 
                {
                    SetFigureImageByCoords((y, x), curCell.Figure.Sprite);
                }
                else
                {
                    SetFigureImageByCoords((y, x), Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero));
                }
            }
        }

        try//throws exception only before first figure selection because selectedCoords = null
        {
            RenderMoves(GameController.GetActualMoves(GameController.selectedCoords.Value));
            RenderAttacks(GameController.GetActualAttacks(GameController.selectedCoords.Value));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    void Start()
    {
        GameController.OnSelected += UpdateBoard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
