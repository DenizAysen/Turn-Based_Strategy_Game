using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractDistance = 1;
    private GridPosition _offsetGridPos;
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
    }
    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();

        for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++)
        {
            for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++)
            {
                _offsetGridPos = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + _offsetGridPos;

                if (!(LevelGrid.Instance.IsValidGridPosition(testGridPosition)))
                    continue;

                Door door = LevelGrid.Instance.GetDoorAtGridPosition(testGridPosition);

                if (door == null)
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Door door = LevelGrid.Instance.GetDoorAtGridPosition(gridPosition);
        door.Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }
    private void OnInteractComplete()
    {
        ActionComplete();
    }
}
