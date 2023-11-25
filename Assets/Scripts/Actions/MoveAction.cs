using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    #region EventHandler
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event EventHandler<OnChangeFloorsStartedEventArg> OnChangedFloorsStarted;
    public class OnChangeFloorsStartedEventArg: EventArgs
    {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }
    #endregion

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
    private bool isChangingFloors;
    private float _differentFloorsTeleportTimer;
    private float _differentFloorsTeleportTimerMax = .5f;
    #endregion

    private void Update()
    {
        if (!_isActive)
            return;

        Vector3 _targetPosition = positionList[_currentPositionIndex];

        if (isChangingFloors)
        {
            _differentFloorsTeleportTimer -= Time.deltaTime;
            Vector3 targetSameFloorPosition = _targetPosition;
            targetSameFloorPosition.y = transform.position.y;
            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * _rotateSpeed);
            if (_differentFloorsTeleportTimer <= 0f)
            {
                isChangingFloors = false;
                transform.position = _targetPosition;
            }
        }
        else
        {
            
            _moveDirection = (_targetPosition - transform.position).normalized;

            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * _rotateSpeed);

            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;
        }       

        if (Vector3.Distance(transform.position, _targetPosition) < stoppingDistance)
        {           
                      
            _currentPositionIndex++;
            if(_currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            else
            {
                _targetPosition = positionList[_currentPositionIndex];
                GridPosition targetGridPostion = LevelGrid.Instance.GetGridPosition(_targetPosition);
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

                if(targetGridPostion.floor != unitGridPosition.floor)
                {
                    isChangingFloors = true;
                    _differentFloorsTeleportTimer = _differentFloorsTeleportTimerMax;

                    OnChangedFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArg { unitGridPosition = unitGridPosition, targetGridPosition = targetGridPostion });
                }
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
                for (int floor = -maxMoveDistance; floor < maxMoveDistance; floor++)
                {
                    _offsetGridPos = new GridPosition(x, z, floor);
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