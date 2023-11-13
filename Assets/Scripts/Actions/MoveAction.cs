using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public EventHandler OnStartMoving;
    public EventHandler OnStopMoving;

    #region Serialized Fields
    [SerializeField] private float stoppingDistance;
    [SerializeField] private int maxMoveDistance = 5;
    #endregion

    #region Privates
    private List<Vector3> positionList;
    private int _currentPositionIndex;
    private Vector3 _moveDirection;
    private float _rotateSpeed = 10f;
    private GridPosition _offsetGridPos;   
    #endregion

    private void Update()
    {
        if (!_isActive)
            return;

        Vector3 _targetPosition = positionList[_currentPositionIndex];

        _moveDirection = (_targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, _moveDirection, Time.deltaTime * _rotateSpeed);

        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance)
        {           
            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;            
        }
        else
        {
            _currentPositionIndex++;
            if(_currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }            
        }       
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(_unit.GetGridPosition(), gridPosition, out int pathLength);
        _currentPositionIndex = 0;
        positionList = new List<Vector3>();
        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                _offsetGridPos = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + _offsetGridPos;

                if (!(LevelGrid.Instance.IsValidGridPosition(testGridPosition))) 
                    continue;

                if (unitGridPosition == testGridPosition)
                    continue;

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue;

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    continue;

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    continue;

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = _unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = targetCountAtGridPosition*10 };
    }
}