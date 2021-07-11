using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UI;

public static class CellFabric
{
    public static void RegisterCell(GameObject cellGameObject, (int, int) coords)//!coords should be written in (y, x) format
    {
        var cell = cellGameObject.AddComponent<Cell>();
        cellGameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameController.OnTrySelectCoords.Invoke(coords);
        });
        GameController.CellsGameObjects[coords.Item1, coords.Item2] = cellGameObject;
    }
}
