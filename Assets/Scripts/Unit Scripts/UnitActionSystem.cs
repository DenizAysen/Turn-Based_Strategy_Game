using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitActionSystem : MonoBehaviour
{
    private bool _isBusy;
    private BaseAction selectedAction;

    #region Singleton
    public static UnitActionSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    #endregion

    #region Events
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted; 
    #endregion

    #region Serialized Fields

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    #endregion

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }
    private void Update()
    {
        if (_isBusy)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleUnitSelection())
        {
            return;
        }

        HandleSelectedAction();
    }
    private bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (selectedUnit == unit)
                        return false;

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        
        return false;
    }
    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!(selectedAction.IsValidActionGridPosition(mouseGridPosition)))
                return;

            if (!(selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)))
                return;

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }
    private void SetBusy()
    {
        _isBusy = true;

        OnBusyChanged?.Invoke(this, _isBusy);
    }
    private void ClearBusy()
    {
        _isBusy = false;

        OnBusyChanged?.Invoke(this, _isBusy);
    }
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        SetSelectedAction(unit.GetMoveAction());

        OnSelectedUnitChanged?.Invoke(this,EventArgs.Empty);
    }
    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

}
