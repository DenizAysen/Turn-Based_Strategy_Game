using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    #region Serialized Fields

    [SerializeField] private float stoppingDistance;
    [SerializeField] private Animator unitAnimator;

    #endregion

    #region Privates

    private Vector3 _targetPosition;
    private Vector3 _moveDirection;
    private float _rotateSpeed = 10f;
    private GridPosition _gridPosition;
    #endregion

    private void Awake()
    {
        _targetPosition = transform.position;
    }
    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
    }
    private void Update()
    {

        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance)
        {
            _moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward,_moveDirection,Time.deltaTime * _rotateSpeed);
            unitAnimator.SetBool("IsWalking", true);
        }
        else 
            unitAnimator.SetBool("IsWalking", false);

        //if (Input.GetKeyDown(KeyCode.T))
        //{ 
        //    Move(new Vector3(4, 0, 4));

        GridPosition _newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if(_newGridPosition != _gridPosition)
        {
            LevelGrid.Instance.UnitMovedFromGridPosition(this, _gridPosition, _newGridPosition);
            _gridPosition = _newGridPosition;
        }
    }
    public void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
       // _targetPosition = new Vector3(_targetPosition.x, 0, _targetPosition.z);
    }
}
