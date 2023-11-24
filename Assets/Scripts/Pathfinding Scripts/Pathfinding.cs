using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathfindingLinkContainer;
    #endregion
    #region Privates
    private const int MOVE_STARIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private int _width;
    private int _height;
    private float _cellSize;
    private int _floorAmount;
    private List<GridSystem<PathNode>> _gridSystemList;
    private List<PathfindingLink> pathfindingLinkList;
    #endregion
    #region Singleton
    public static Pathfinding Instance { get; private set; }
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
    public void Setup(int width,int height, float cellSize, int floorAmount)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _floorAmount = floorAmount;

        _gridSystemList = new List<GridSystem<PathNode>>();

        for (int floor = 0; floor < _floorAmount; floor++)
        {
            GridSystem<PathNode> _gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize, floor, LevelGrid.FLOOR_HEIGHT,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            _gridSystemList.Add(_gridSystem);
        }
        
       // _gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                for (int floor = 0; floor < _floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float raycastOffsetDistance = 1f;
                    GetNode(x, z, floor).SetIsWalkable(false);
              
                    if (Physics.Raycast(worldPosition + Vector3.up * raycastOffsetDistance, Vector3.down, raycastOffsetDistance * 2, floorLayerMask))
                    {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }

                    if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }               
            }
        }

        pathfindingLinkList = new List<PathfindingLink>();
        foreach (Transform pathfindingLinkTransform in pathfindingLinkContainer)
        {
            if(pathfindingLinkTransform.TryGetComponent(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour))
            {
                pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathFindingLink());
            }
        }
    }
    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLenght)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                for (int floor = 0; floor < _floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.RestCameFromPathNode();
                }
                
            }
        }
        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition,endGridPosition));
        startNode.CalculateFCost();

        PathNode currentNode;
        while(openList.Count > 0)
        {
            currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLenght = endNode.GetFCost();
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());
                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();
                }
                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }
        }
        pathLenght = 0;
        return null;
    }
    public int CalculateDistance(GridPosition gridPosA, GridPosition gridPosB)
    {
        GridPosition gridPositionDistance = gridPosA - gridPosB;
        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return Mathf.Min(xDistance,zDistance)* MOVE_DIAGONAL_COST + MOVE_STARIGHT_COST* remaining;
    }
    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }
    private GridSystem<PathNode> GetGridSystem(int floor)
    {
        return _gridSystemList[floor];
    }
    private PathNode GetNode(int x, int z, int floor)
    {
       return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x -1 >= 0)
        {
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z, gridPosition.floor));
            if(gridPosition.z - 1 >= 0)
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
            }
            if(gridPosition.z + 1 < _height)
            {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
            }
            
        }

        if(gridPosition.x + 1 < _width)
        {
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z, gridPosition.floor));
            if (gridPosition.z - 1 >= 0)
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
            if (gridPosition.z + 1 < _height)
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
        }

        if (gridPosition.z - 1 >= 0)
            neighbourList.Add(GetNode(gridPosition.x, gridPosition.z - 1, gridPosition.floor));
        if (gridPosition.z + 1 < _height)
            neighbourList.Add(GetNode(gridPosition.x , gridPosition.z + 1, gridPosition.floor));

        List<PathNode> totalNeighborList = new List<PathNode>();
        totalNeighborList.AddRange(neighbourList);

        List<GridPosition> pathfindingLinkGridPositionList = GetPathFindingLinkConnectedGridPositionList(gridPosition);

        foreach (GridPosition pathfindingLinkGridPosition in pathfindingLinkGridPositionList)
        {
            totalNeighborList.Add(GetNode(
                pathfindingLinkGridPosition.x, 
                pathfindingLinkGridPosition.z, 
                pathfindingLinkGridPosition.floor
                )
                );
        }

        return totalNeighborList;
    }
    private List<GridPosition> GetPathFindingLinkConnectedGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathfindingLink pathfindingLink in pathfindingLinkList)
        {
            if(pathfindingLink.gridPositionA == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionList;
    }
    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathNodeList.Reverse();
        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }
        return gridPositionList;
    }
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }
    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition,out int pathLength) != null;
    }
    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
