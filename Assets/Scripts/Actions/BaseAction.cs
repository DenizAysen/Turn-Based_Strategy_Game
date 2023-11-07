using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static EventHandler OnAnyActionStarted;
    public static EventHandler OnAnyActionCompleted;

    protected Unit _unit;
    protected bool _isActive;
    protected Action OnActionComplete;
    protected virtual void Awake()
    {
        _unit = GetComponent<Unit>();
    }
    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetValidActionGridPositionList();
        return validGridPositions.Contains(gridPosition);
    }
    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }
    protected void ActionStart(Action OnActionComplete)
    {
        _isActive = true;
        this.OnActionComplete = OnActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }
    protected void ActionComplete()
    {
        _isActive = false;
        OnActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }
    public Unit GetUnit()
    {
        return _unit;
    }
}
