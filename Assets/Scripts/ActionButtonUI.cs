using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    [SerializeField] private Image selectedImage;

    private BaseAction _baseAction;
    public void SetBaseAction(BaseAction baseAction)
    {
        _baseAction = baseAction;
        buttonText.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }
    public void UpdateSelectedVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedAction() == _baseAction)
            selectedImage.gameObject.SetActive(true);
        else
            selectedImage.gameObject.SetActive(false);
    }
}
