using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    #region Serialized Fields

    [SerializeField] private float stoppingDistance; 

    #endregion

    #region Privates

    private Vector3 _targetPosition;
    private Vector3 _moveDirection; 

    #endregion

    private void Update()
    {
        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance)
        {
            _moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += _moveDirection * moveSpeed * Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        { 
            Move(new Vector3(4, 0, 4));
        }
    }
    private void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }
}
