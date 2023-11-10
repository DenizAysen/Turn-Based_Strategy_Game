using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    #region Privates

    private GridSystem<GridObject> _gridSystem;
    private GridPosition _gridPosition;
    private List<Unit> _unitList; 

    #endregion
    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in _unitList)
        {
            unitString += unit + "\n";
        }
        return _gridPosition.ToString() + "\n" + unitString;
    }

    public List<Unit> GetUnitList()
    {
        return _unitList;
    }
    public void AddUnit(Unit unit)
    {
        _unitList.Add(unit);
    }
    public void RemoveUnit(Unit unit)
    {
        _unitList.Remove(unit);
    }
    public bool HasAnyUnit() => _unitList.Count > 0;
    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return _unitList[0];
        }
        else
            return null;
    }
}
