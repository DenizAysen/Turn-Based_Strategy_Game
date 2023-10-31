using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    #region Singleton
    public static GridSystemVisual Instance;

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

    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition),Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
       // HideAllGridPositions();
    }
    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    public void ShowGridPositionList(List<GridPosition> gridPositions )
    {
        foreach (GridPosition gridPosition in gridPositions)
        {
            Debug.Log(gridPosition.ToString());
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
        }
    }
    public void UpdateGridVisual()
    {
        HideAllGridPositions();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList());
    }
    private void Update()
    {
        UpdateGridVisual();
    }
}
