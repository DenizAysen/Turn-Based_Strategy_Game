using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractionComplete;
    private bool _isActive;
    private float _timer;

    [SerializeField] private bool isOpen;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        CloseDoor();
    }
    private void Update()
    {
        if (!_isActive)
            return;

        _timer -= Time.deltaTime;

        if(_timer <= 0)
        {
            _isActive = false;
            onInteractionComplete?.Invoke();
        }
    }
    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;

        _isActive = true;
        _timer = .5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();           
        }
    }
    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }
    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }
}
