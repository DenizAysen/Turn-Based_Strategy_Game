using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    #region Serialized Fields

    [SerializeField] private float stoppingDistance;
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 5;
    #endregion

    #region Privates

    private Vector3 _targetPosition;
    private Vector3 _moveDirection;
    private float _rotateSpeed = 10f;
    private GridPosition _offsetGridPos;
    
    #endregion
    protected override void Awake()
    {
        _targetPosition = transform.position;
        base.Awake();
    }

    private void Update()
    {
        if (!_isActive)
            return;

        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance)
        {
            _moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, _moveDirection, Time.deltaTime * _rotateSpeed);
            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking", false);
            _isActive = false;
            onActionComplete();
        }
            
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        _targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        _isActive = true;
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

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}