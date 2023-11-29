using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    #region Privates

    private Camera _camera;
    private static MouseWorld instance;
    #endregion

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _camera = Camera.main;
    }
    //private void Update()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit));
    //    transform.position = raycastHit.point;
    //}

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
    public static Vector3 GetPositionOnlyHitVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, float.MaxValue, instance.mousePlaneLayerMask);
        System.Array.Sort(raycastHits, (RaycastHit raycastHitA, RaycastHit raycastHitB) => 
        {
            return Mathf.RoundToInt(raycastHitA.distance)  - Mathf.RoundToInt(raycastHitB.distance);
        });
        foreach (RaycastHit raycastHit in raycastHits)
        {
            if(raycastHit.transform.TryGetComponent(out Renderer renderer))
            {
                if (renderer.enabled)
                    return raycastHit.point;
            }
        }
        return Vector3.zero;
    }
}
