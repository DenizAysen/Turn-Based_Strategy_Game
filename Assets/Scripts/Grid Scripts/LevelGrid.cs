using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    #region Serialized Fields
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _cellSize;
    [SerializeField] private int floorAmount;
    #endregion

    public event EventHandler OnAnyUnitMovedGridPosition;
    public const float FLOOR_HEIGHT = 3f;
    private  List<GridSystem<GridObject>> gridSystemList;
    
    #region Singleton
    public static LevelGrid Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystemList = new List<GridSystem<GridObject>>();
        for (int floor = 0; floor < floorAmount; floor++)
        {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize, floor, FLOOR_HEIGHT, 
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            gridSystemList.Add(gridSystem);
        }
        //gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize,(GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    #endregion
    private void Start()
    {
        Pathfinding.Instance.Setup(_width, _height, _cellSize, floorAmount);
    }
    private GridSystem<GridObject> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).AddUnit(unit);
    }
    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitList();
    }
    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveUnit(unit);
    }
    public void UnitMovedFromGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public int GetFloor(Vector3 worldPosition)
    {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }
    public GridPosition GetGridPosition(Vector3 worldPos) 
    {
        int floor = GetFloor(worldPos);
        return GetGridSystem(floor).GetGridPosition(worldPos); 
    }
    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        if(gridPosition.floor <0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }
        else
        {
            return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
        }
    }
    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeight() => GetGridSystem(0).GetHeight();
    public int GetFloorAmount() => floorAmount;
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }
    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }
}
