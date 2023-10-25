using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class UnitSelectedVisual : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private Unit unit;

    #endregion

    #region Privates

    private MeshRenderer _meshRenderer;

    #endregion

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;

        UpdateVisual();
    }

    private void OnSelectedUnitChanged(object sender , EventArgs empty)
    {
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
            _meshRenderer.enabled = true;
        else
            _meshRenderer.enabled = false;
    }
}
