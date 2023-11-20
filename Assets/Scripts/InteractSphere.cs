using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour , IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private bool isGreen;
    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool _isActive;
    private float _timer;
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColorGreen();
    }
    private void Update()
    {
        if (!_isActive)
            return;

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _isActive = false;
            onInteractionComplete?.Invoke();
        }
    }
    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }
    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        _isActive = true;
        _timer = .5f;

        if (isGreen)
            SetColorRed();
        else
            SetColorGreen();
    }
}
