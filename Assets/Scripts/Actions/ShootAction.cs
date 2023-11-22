using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }
    private enum States
    {
        Aiming,
        Shooting,
        Cooloff
    }

    #region Privates
    private int _maxShootDistance = 7;
    private GridPosition _offsetGridPos;
    private States _state;
    private float _stateTimer;
    private Unit _targetUnit;
    private bool _canShootBullet;
    private float rotateSpeed = 10f;
    private Vector3 _shootDirection;
    private float _unitShoulderHeight = 1.7f;
    #endregion
    [SerializeField] private LayerMask obstaclesLayerMask;
    private void Update()
    {
        if (!_isActive)
            return;

        _stateTimer -= Time.deltaTime;

        switch (_state)
        {
            case States.Aiming:
                _shootDirection = (_targetUnit.GetWorldPosition() - _unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, _shootDirection, rotateSpeed * Time.deltaTime);
                break;
            case States.Shooting:
                if (_canShootBullet)
                {
                    Shoot();
                    _canShootBullet = false;
                }                   
                break;
            case States.Cooloff:              
                break;
        }
        if(_stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = _targetUnit, shootingUnit = _unit });
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = _targetUnit , shootingUnit = _unit });
        _targetUnit.Damage(40);
    }

    private void NextState()
    {
        switch (_state)
        {
            case States.Aiming:
                _state = States.Shooting;
                float shootingStateTimer = .1f;
                _stateTimer = shootingStateTimer;
                break;
            case States.Shooting:
                _state = States.Cooloff;
                float coolOffStateTimer = .5f;
                _stateTimer = coolOffStateTimer;
                break;
            case States.Cooloff:               
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = _unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
     
        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                _offsetGridPos = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + _offsetGridPos;

                if (!(LevelGrid.Instance.IsValidGridPosition(testGridPosition)))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > _maxShootDistance)
                    continue;

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == _unit.IsEnemy())
                {
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                if(Physics.Raycast(unitWorldPosition + Vector3.up * _unitShoulderHeight, 
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask))
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        _state = States.Aiming;
        float aimingStateTimer = 1f;
        _stateTimer = aimingStateTimer;
        _canShootBullet = true;

        ActionStart(onActionComplete);
    }
    public Unit GetTargetUnit()
    {
        return _targetUnit;
    }
    public int GetMaxShootDistance()
    {
        return _maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 100 + Mathf.RoundToInt((1- targetUnit.GetHealthNormalized()) * 100f)};
    }
    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
