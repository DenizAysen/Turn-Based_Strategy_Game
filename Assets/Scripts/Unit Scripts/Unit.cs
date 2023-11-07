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
    private HealthSystem _healthSystem;
    #endregion

    public static EventHandler OnAnyActionPointsChanged;

    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        _moveAction = GetComponent<MoveAction>();
        _spinAction = GetComponent<SpinAction>();
        baseActions = GetComponents<BaseAction>();
        _healthSystem = GetComponent<HealthSystem>();
    }
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        _healthSystem.OnDead += OnDead;
    }

    private void OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this);
        Destroy(gameObject);
    }

    private void OnTurnChanged(object sender, EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }        
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
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
    public BaseAction[] GetBaseActions()
    {
        return baseActions;
    }
    public bool IsEnemy()
    {
        return isEnemy;
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
    public void Damage(int damageAmount)
    {
        _healthSystem.Damage(damageAmount);
    }
}
