using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GridDebugObject : MonoBehaviour
{
    private GridObject _gridObject;
    [SerializeField] private TextMeshPro positionText;
    public void SetGridObject(GridObject gridObject)
    {
        _gridObject = gridObject;
        positionText.text = gridObject.ToString();
    }
}
