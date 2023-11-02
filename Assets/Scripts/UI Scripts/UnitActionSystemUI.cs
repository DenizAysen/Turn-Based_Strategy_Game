using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIs;
    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
    }
    private void Start()
    {
        SubscribeEvents();

        CreateUnitActionButtons();
        UpdateActionPoints();
        UpdateSelectedVisual();
    }
    private void SubscribeEvents()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;
    }

    private void OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIs.Clear();

        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in unit.GetBaseActions())
        {
            Transform buttonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = buttonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIs.Add(actionButtonUI);
        }
    }
    private void OnSelectedUnitChanged(object sender,EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    private void OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }
    private void OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButton in actionButtonUIs)
        {
            actionButton.UpdateSelectedVisual();
        }
    }
    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }
}
