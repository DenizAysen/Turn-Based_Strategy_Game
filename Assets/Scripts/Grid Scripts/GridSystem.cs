using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject> 
{
    private int _width;
    private int _height;
    private float _cellSize;
    private int _floor;
    private float _floorHeight;
    private TGridObject[,] gridObjectArray;
    public GridSystem(int width, int height,float cellSize, int floor, float floorHeight, Func<GridSystem<TGridObject>, GridPosition,TGridObject > createGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _floor = floor;
        _floorHeight = floorHeight;

        gridObjectArray = new TGridObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, _floor);
               gridObjectArray[x,z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize 
            + new Vector3(0,gridPosition.floor,0) * _floorHeight;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Convert.ToInt32(worldPosition.x/_cellSize), Convert.ToInt32(worldPosition.z / _cellSize), _floor);
    }
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, _floor);
                Transform debugTransform = GameObject.Instantiate(debugPrefab,GetWorldPosition(gridPosition),Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.z >= 0 && 
               gridPosition.x < _width && 
               gridPosition.z < _height && 
               gridPosition.floor == _floor;
    }
    public int GetWidth()
    {
        return _width;
    }
    public int GetHeight()
    {
        return _height;
    }
}
