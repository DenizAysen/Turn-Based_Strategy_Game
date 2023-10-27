using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Privates  
    private GridPosition _gridPosition;
    private MoveAction _moveAction;
    #endregion

    private void Awake()
    {
        _moveAction = GetComponent<MoveAction>();
    }
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
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
    public MoveAction GetMoveAction()
    {
        return _moveAction;
    }
    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }
}
