using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    #region Privates  
    private GridPosition _gridPosition;
    private MoveAction _moveAction;
    private SpinAction _spinAction;
    private BaseAction[] baseActions;
    private int actionPoints = ACTION_POINTS_MAX;
    #endregion

    public static EventHandler OnAnyActionPointsChanged;

    private void Awake()
    {
        _moveAction = GetComponent<MoveAction>();
        _spinAction = GetComponent<SpinAction>();
        baseActions = GetComponents<BaseAction>();
    }
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, EventArgs e)
    {
        actionPoints = ACTION_POINTS_MAX;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition _newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if(_newGridPosition != _gridPosition)
        {
            LevelGrid.Instance.UnitMovedFromGridPosition(this, _gridPosition, _newGridPosition);
            _gridPosition = _newGridPosition;
        }
    }
    public int GetActionPoints()
    {
        return actionPoints;
    }
    public MoveAction GetMoveAction()
    {
        return _moveAction;
    }
    public SpinAction GetSpinAction()
    {
        return _spinAction;
    }
    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }
    public BaseAction[] GetBaseActions()
    {
        return baseActions;
    }
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }
}
