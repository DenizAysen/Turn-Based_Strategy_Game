using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public EventHandler<OnShootEventArgs> OnShoot;
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
    #endregion

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
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition();
        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                _offsetGridPos = new GridPosition(x, z);
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

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        _state = States.Aiming;
        float aimingStateTimer = 1f;
        _stateTimer = aimingStateTimer;
        _canShootBullet = true;
    }
}
