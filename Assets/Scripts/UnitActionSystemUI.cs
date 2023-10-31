using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private ActionBusyUI busyUI;

    private List<ActionButtonUI> actionButtonUIs;
    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;

        CreateUnitActionButtons();

        UpdateSelectedVisual();
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
    }
    private void OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }
    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButton in actionButtonUIs)
        {
            actionButton.UpdateSelectedVisual();
        }
    }
}
