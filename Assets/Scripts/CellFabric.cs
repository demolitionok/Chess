using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UI;

public static class CellFabric
{
    public static Cell CreateCell(GameObject cellGameObject, (int, int) coords)
    {
        var cell = cellGameObject.AddComponent<Cell>();
        cellGameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameController.OnTrySelectCoords.Invoke(coords);
        });
        GameController.CellsGameObjects[coords.Item1, coords.Item2] = cellGameObject;
        return cell;
    }
}
