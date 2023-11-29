using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    private GridPosition _offsetGridPos;
    private int _maxThrowDistance = 7;

    public event EventHandler<OnGrenadeEventArgs> OnGrenadeActionStarted;
    public event EventHandler OnGrenadeActionCompleted;
    public class OnGrenadeEventArgs : EventArgs
    {
        public GridPosition targetGridPosition;
        public Action onGrenadeBehaviourComplete;
    }

    [SerializeField]private Transform grenadeProjectilePrefab;
    private Vector3 _targetPosition;
    private Vector3 _throwDirection;
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        _throwDirection = (_targetPosition - _unit.GetWorldPosition()).normalized;
        transform.forward = Vector3.Slerp(transform.forward, _throwDirection, 10 * Time.deltaTime);
    }
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();

        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++)
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                _offsetGridPos = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + _offsetGridPos;

                if (!(LevelGrid.Instance.IsValidGridPosition(testGridPosition)))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > _maxThrowDistance)
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Transform grenadeProjectileTrnasform = Instantiate(grenadeProjectilePrefab, _unit.GetWorldPosition(), Quaternion.identity);
        //GrenadeProjectile grenadeProjectile = grenadeProjectileTrnasform.GetComponent<GrenadeProjectile>();
        //grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        _targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        OnGrenadeActionStarted?.Invoke(this, new OnGrenadeEventArgs { targetGridPosition = gridPosition, onGrenadeBehaviourComplete = OnGrenadeBehaviourComplete});
        ActionStart(onActionComplete);
    }
    private void OnGrenadeBehaviourComplete()
    {
        OnGrenadeActionCompleted?.Invoke(this, EventArgs.Empty);
        ActionComplete();
    }
}
