using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    #region privates
    CinemachineTransposer _cinemachineTransposer;
    private Vector3 _targetFollowOffset;
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;
    float _zoomSpeed = 5f;
    #endregion

    #region Singleton
    public static CameraController Instance { get; private set; }

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

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
   
    private void Start()
    {
        _cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _targetFollowOffset = _cinemachineTransposer.m_FollowOffset;
    }
    private void Update()
    {
        MoveCamera();

        RotateCamera();

        HandleZoom();
    }
    private void MoveCamera()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
        
        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }
    private void RotateCamera()
    {
        Vector3 rotationVector = Vector3.zero;

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    private void HandleZoom()
    {
        //float zoomIncreaseAmount = 1f;
        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();
        
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * _zoomSpeed);
    }
    public float GetCameraHeight()
    {
        return _targetFollowOffset.y;
    }
}
