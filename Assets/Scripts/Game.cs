using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Normal,
    Check,
    Mate
}

public class Game : MonoBehaviour
{
    private GameController GameController;
    private Board Board;
    private BoardRenderer BoardRenderer;
    public GameObject CellPrefab;
    public int xSize;
    public int ySize;
    private float cellWidth;
    private float cellHeight;
    public float Offset;


    public void Start()
    {
        cellWidth = CellPrefab.GetComponent<RectTransform>().rect.width;
        cellHeight = CellPrefab.GetComponent<RectTransform>().rect.height;
        Board = new Board(ySize, xSize);
        GameController = new GameController(Board.GetCellByCoords);
        BoardRenderer = new BoardRenderer(Board.GetCellByCoords);
        GenerateBoard();
        UpdateBoard();
        GameController.OnSelection += UpdateBoard;
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                GameObject cellGameObject = Instantiate(CellPrefab,
                    new Vector3(x * cellWidth + Offset, -y * cellHeight - Offset), Quaternion.identity);
                cellGameObject.transform.SetParent(gameObject.transform, false);

                Board.RegisterCell(cellGameObject, (y, x), GameController.OnTrySelectCoords);

                cellGameObject = Board.GetCellByCoords((y, x)).gameObject;

                var cell = cellGameObject.GetComponent<Cell>();
                cell.Figure = null;

                if (y == 1)
                {
                    cell.Figure = new Pawn("pawn", Side.Black);
                }

                if (y == 6)
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
            }
        }
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
                BoardRenderer.ColorButton(curCellButton, GameController.selectedCoords == (y, x) ? Color.green : Color.white);
                if (curCell.Figure != null)
                {
                    BoardRenderer.SetFigureImageByCoords((y, x), curCell.Figure.Sprite);
                }
                else
                {
                    BoardRenderer.SetFigureImageByCoords((y, x), Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero));
                }
            }
        }

        try //throws exception only before first figure selection because selectedCoords = null
        {
            BoardRenderer.RenderMoves(GameController.GetActualMoves(GameController.selectedCoords.Value));
            BoardRenderer.RenderAttacks(GameController.GetActualAttacks(GameController.selectedCoords.Value));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}