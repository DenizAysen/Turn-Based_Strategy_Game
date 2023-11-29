using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] private bool dynamicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRendererList;

    private Renderer[] rendererArray;
    private int _floor;
    private float _floorHeightOffset = 2f;
    private Unit unit;
    float _cameraHeight;
    bool _showObject;
    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in rendererArray)
        {
            renderer.enabled = true;
        }
    }
    private void Start()
    {
        _floor = LevelGrid.Instance.GetFloor(transform.position);

        if (_floor == 0 && dynamicFloorPosition)
            Destroy(this);
    }
    private void Update()
    {
        ShowOrHideObjects();
    }
    private void ShowOrHideObjects()
    {
        if (dynamicFloorPosition)
        {
            _floor = LevelGrid.Instance.GetFloor(transform.position);
        }

        _cameraHeight = CameraController.Instance.GetCameraHeight();

        _showObject = _cameraHeight > LevelGrid.FLOOR_HEIGHT * _floor + _floorHeightOffset;

        if (_showObject || _floor == 0)
            Show();
        else
            Hide();
    }
    private void Show()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = true;
        }
    }
    private void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = false;
        }
    }
}
