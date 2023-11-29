using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    #region SerializedFields
    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    [SerializeField] private bool canMove;
    #endregion

    #region Privates
    private Vector3 _targetPositon;
    private Vector3 _moveDir;
    private Vector3 _positionXZ;
    private float _positionY;
    private float _moveSpeed = 15f;
    private float _maxHeight;
    private Action onGrenadeBehaviourComplete;
    private float _reachedTargetDistance = .2f;
    private float _totalDistance;
    #endregion
    private void Update()
    {
        if (!canMove)
            return;

        _moveDir = (_targetPositon - _positionXZ).normalized;

        _positionXZ += _moveDir * _moveSpeed * Time.deltaTime;
        float distance = Vector3.Distance(_positionXZ, _targetPositon);
        float distanceNormalized = 1 - distance / _totalDistance;

        _maxHeight = _totalDistance / 4f;
        _positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * _maxHeight;
        transform.position = new Vector3(_positionXZ.x, _positionY, _positionXZ.z);
        if (Vector3.Distance(_positionXZ, _targetPositon) < _reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliders = Physics.OverlapSphere(_targetPositon, damageRadius);
            foreach (Collider col in colliders)
            {
                if(col.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
                if (col.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVfxPrefab, _targetPositon + Vector3.up , Quaternion.identity);
            Destroy(gameObject);

            onGrenadeBehaviourComplete();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        _targetPositon = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        _positionXZ = transform.position;
        _positionXZ.y = 0;
        _totalDistance = Vector3.Distance(_positionXZ, _targetPositon);
    }
    public void EnableProjectileMovement()
    {
        canMove = true;
    }
}
