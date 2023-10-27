using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
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
    private Unit _unit;
    #endregion
    private void Awake()
    {
        _targetPosition = transform.position;
        _unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance)
        {
            _moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, _moveDirection, Time.deltaTime * _rotateSpeed);
            unitAnimator.SetBool("IsWalking", true);
        }
        else
            unitAnimator.SetBool("IsWalking", false);
    }
    public void Move(GridPosition gridPosition)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    }
    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetValidActionGridPositionList();
        return validGridPositions.Contains(gridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList()
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
                Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }
}