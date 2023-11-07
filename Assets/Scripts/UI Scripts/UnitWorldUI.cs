using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthbarImage;
    [SerializeField] private HealthSystem healthSystem;
    #endregion
    private void Start()
    {
        SubscribeEvents();
        UpdateActionPointsText();
        UpdateHealthBar();
    }
    private void SubscribeEvents()
    {
        Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;
        healthSystem.OnDamaged += OnDamaged;
    }

    private void OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }
    private void UpdateHealthBar()
    {
        healthbarImage.fillAmount = healthSystem.GetHealthNormalized();
    }
}
