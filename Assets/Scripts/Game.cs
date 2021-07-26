using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GameState
{
    Normal,
    Check,
    Mate
}

public class Game : MonoBehaviour
{
    [SerializeField]
    private GameObject CellPrefab;
    [SerializeField]
    private int xSize;
    [SerializeField]
    private int ySize;
    [SerializeField]
    private float offset;
    
    private float cellWidth;
    private float cellHeight;
    private GameController gameController;
    private InputController inputController;
    private Board Board;


    public void Start()
    {
        cellWidth = CellPrefab.GetComponent<RectTransform>().rect.width;
        cellHeight = CellPrefab.GetComponent<RectTransform>().rect.height;
        Board = new Board(ySize, xSize);
        gameController = new GameController(Board.GetCellByCoords, ySize, xSize);
        inputController = new InputController(gameController.GetCurrentSide, gameController.GetSelectedCoords,
            Board.GetCellByCoords);

        inputController.ActionRequest += gameController.ProcessAction;
        gameController.OnSelection += UpdateBoard;
        GenerateBoard();
        UpdateBoard();
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var cell = CreateCell((y, x));

                if (y == 1)
                {
                    cell.Figure = new Pawn("pawn", Side.Black);
                }

                if (y == ySize - 2)
                {
                    cell.Figure = new Pawn("pawn", Side.White);
                }

                if (y == 0 && (x == 0 || x == xSize - 1))
                {
                    cell.Figure = new Tower("tower", Side.Black);
                }

                if (y == ySize - 1 && (x == 0 || x == xSize - 1))
                {
                    cell.Figure = new Tower("tower", Side.White);
                }

                if (y == 0 && (x == 2 || x == xSize - 3))
                {
                    cell.Figure = new Bishop("bishop", Side.Black);
                }

                if (y == ySize - 1 && (x == 2 || x == xSize - 3))
                {
                    cell.Figure = new Bishop("bishop", Side.White);
                }

                if (y == 0 && x == xSize - 5)
                {
                    cell.Figure = new Queen("queen", Side.Black);
                }

                if (x == 3 && y == ySize - 1)
                {
                    cell.Figure = new Queen("queen", Side.White);
                }

                if (y == 0 && (x == 1 || x == xSize - 2))
                {
                    cell.Figure = new Knight("knight", Side.Black);
                }

                if (y == ySize - 1 && (x == 1 || x == xSize - 2))
                {
                    cell.Figure = new Knight("knight", Side.White);
                }

                if (y == 0 && (x == 4 || x == xSize - 4))
                {
                    cell.Figure = new King("king", Side.Black);
                }

                if (y == ySize - 1 && (x == 4 || x == xSize - 4))
                {
                    cell.Figure = new King("king", Side.White);
                }
                
                Board.RegisterCell(cell, (y, x), inputController.RequestAction);
            }
        }
    }

    private Cell CreateCell((int,int) coords)
    {
        GameObject cellGameObject = Instantiate(CellPrefab,
            new Vector3(coords.Item2 * cellWidth + offset, -coords.Item1 * cellHeight - offset), Quaternion.identity);
        cellGameObject.transform.SetParent(gameObject.transform, false);
        var cell = cellGameObject.AddComponent<Cell>();
        return cell;
    }

    public void UpdateBoard()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var curCell = Board.GetCellByCoords((y, x));
                //SetCellInfoText((y,x));
                var curCellButton = curCell.gameObject.GetComponent<Button>();
                BoardRenderer.ColorButton(curCellButton,
                    gameController.GetSelectedCoords() == (y, x) ? Color.green : Color.white);
                BoardRenderer.SetFigureImageByCoords((y, x),
                    curCell.Figure != null
                        ? curCell.Figure.Sprite
                        : Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero), Board.GetCellByCoords);
            }
        }

        if (gameController.GetSelectedCoords() != null)
        {
            BoardRenderer.RenderMoves(gameController.GetActualMoves(gameController.GetSelectedCoords().Value),
                Board.GetCellByCoords);
            BoardRenderer.RenderAttacks(gameController.GetActualAttacks(gameController.GetSelectedCoords().Value),
                Board.GetCellByCoords);
        }
    }
}