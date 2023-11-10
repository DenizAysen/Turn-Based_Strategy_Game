using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GridDebugObject : MonoBehaviour
{
    private object _gridObject;
    [SerializeField] private TextMeshPro positionText;
    public virtual void SetGridObject(object gridObject)
    {
        _gridObject = gridObject;
    }
    protected virtual void Update()
    {
        positionText.text = _gridObject.ToString();
    }
}
