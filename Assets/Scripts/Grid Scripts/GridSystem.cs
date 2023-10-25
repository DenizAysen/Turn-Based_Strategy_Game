using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem 
{
    private int _width;
    private int _height;
    private float _cellSize;
    private GridObject[,] gridObjectArray;
    public GridSystem(int width, int height,float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        gridObjectArray = new GridObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
               gridObjectArray[x,z] = new GridObject(this, new GridPosition(x, z));
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Convert.ToInt32(worldPosition.x/_cellSize), Convert.ToInt32(worldPosition.z / _cellSize));
    }
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab,GetWorldPosition(gridPosition),Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }
}
