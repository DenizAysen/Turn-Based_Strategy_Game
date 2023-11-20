using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance { get; private set; }

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
    public Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }
    public bool IsMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }
    public Vector2 GetCameraMoveVector()
    {
        Vector3 tempMoveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            tempMoveDir.z = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            tempMoveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            tempMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            tempMoveDir.x = 1f;
        }

        Vector2 inputMoveDir = new Vector2(tempMoveDir.x, tempMoveDir.z);

        return inputMoveDir;
    }
    public float GetCameraRotateAmount()
    {
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
    }
    public float GetCameraZoomAmount()
    {
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1;
        }

        return zoomAmount;
    }
}
