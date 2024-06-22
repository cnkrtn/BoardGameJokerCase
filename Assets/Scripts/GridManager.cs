using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public enum GridSize { Small, Medium }

    public GridSize gridSize;
    public GameObject gridCellPrefab;
    public GameObject pathPrefab; // Reference to the new prefab
    public Transform gridParent;
    public Transform finalPath;
    
    public bool createSquarePath = true; // New flag to control the square path

    private int rows;
    private int columns;
    private Vector2Int unwalkableCenter;
    private int unwalkableSize;
    private int connectionCount = 0;
    
    private Dictionary<Vector2Int, GameObject> gridCells;
    public List<Vector2Int> waypoints, modifiedWaypoints, finalPathTiles;
    public List<GameObject> finalPathGameObjects;

    void Start()
    {
        gridCells = new Dictionary<Vector2Int, GameObject>();
        waypoints = new List<Vector2Int>();
        modifiedWaypoints = new List<Vector2Int>();
        finalPathTiles = new List<Vector2Int>();
        finalPathGameObjects = new List<GameObject>();

        SetGridSize();
        SetUnwalkableArea();
        CreateGrid();
        CreateSections();
        FindPathsBetweenWaypoints();
        AssignPathTileIndices();
        GameObjectList();
    }

    private void GameObjectList()
    {
        foreach (var tile in finalPathTiles)
        {
            var tileObject = GetGameObjectAtGridPosition(tile);
            finalPathGameObjects.Add(tileObject);
        }
        
        if (createSquarePath)
        {
            finalPathTiles.Add(waypoints[0]);
            var tileObject = GetGameObjectAtGridPosition(waypoints[0]);
            finalPathGameObjects.Add(tileObject);
        }
        else
        {
            finalPathTiles.Add(waypoints[waypoints.Count - 5]);
            var tileObject = GetGameObjectAtGridPosition(waypoints[waypoints.Count - 2]);
            finalPathGameObjects.Add(tileObject);
        }

        foreach (var gameObject in finalPathGameObjects)
        {
            gameObject.transform.parent = finalPath;
        }
    }

    void SetGridSize()
    {
        switch (gridSize)
        {
            case GridSize.Small:
                rows = 11;
                columns = 11;
                break;
            case GridSize.Medium:
                rows = 21;
                columns = 21;
                break;
        }
    }

    void SetUnwalkableArea()
    {
        unwalkableCenter = new Vector2Int(rows / 2, columns / 2);

        switch (gridSize)
        {
            case GridSize.Small:
                unwalkableSize = 5;
                break;
            case GridSize.Medium:
                unwalkableSize = 13;
                break;
        }
    }

    void CreateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2Int gridPosition = new Vector2Int(i, j);
                GameObject cell = Instantiate(gridCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
                cell.transform.parent = gridParent;

                GridObject gridObject = cell.GetComponent<GridObject>();
                if (gridObject != null)
                {
                    gridObject.gridPosition = gridPosition;
                    gridObject.walkable = true;

                    gridCells.Add(gridPosition, cell);

                    if (IsWithinUnwalkableArea(gridObject.gridPosition))
                    {
                        gridObject.walkable = false;
                    }
                    else if (!createSquarePath && IsInPathToFirstRow(gridObject.gridPosition))
                    {
                        gridObject.walkable = false;
                    }
                }
            }
        }
    }

    bool IsWithinUnwalkableArea(Vector2Int position)
    {
        int halfSize = unwalkableSize / 2;
        return position.x >= unwalkableCenter.x - halfSize && position.x <= unwalkableCenter.x + halfSize &&
               position.y >= unwalkableCenter.y - halfSize && position.y <= unwalkableCenter.y + halfSize;
    }

    bool IsInPathToFirstRow(Vector2Int position)
    {
        int halfSize = unwalkableSize / 2;
        return position.x >= unwalkableCenter.x - halfSize && position.x <= unwalkableCenter.x + halfSize && position.y <= unwalkableCenter.y;
    }

    void CreateSections()
    {
        switch (gridSize)
        {
            case GridSize.Small:
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, 0), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, rows - 7), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, rows - 3), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 7, rows - 3), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 3, rows - 3), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 3, rows - 7), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 3, 0), 3));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 7, 0), 3));
                ModifyWaypoints();
                break;
            case GridSize.Medium:
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, 0), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, rows - 12), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(0, rows - 4), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 12, rows - 4), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 4, rows - 4), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 4, rows - 12), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 4, 0), 4));
                waypoints.Add(CreateRandomCellInSection(new Vector2Int(columns - 12, 0), 4));
                ModifyWaypoints();
                break;
        }
    }

    private void ModifyWaypoints()
    {
        var newWaypoint = new Vector2Int();
        for (int i = 0; i < waypoints.Count; i++)
        {
            switch (i)
            {
                case 0:
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y + 1);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 1:
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y - 1);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y + 1);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 2:
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y - 1);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x + 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 3:
                    newWaypoint = new Vector2Int(waypoints[i].x - 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x + 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 4:
                    newWaypoint = new Vector2Int(waypoints[i].x - 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y - 1);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 5:
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y + 1);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y - 1);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 6:
                    newWaypoint = new Vector2Int(waypoints[i].x, waypoints[i].y + 1);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x - 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
                case 7:
                    newWaypoint = new Vector2Int(waypoints[i].x + 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[i].x - 1, waypoints[i].y);
                    modifiedWaypoints.Add(newWaypoint);
                    newWaypoint = new Vector2Int(waypoints[0].x + 1, waypoints[0].y);
                    modifiedWaypoints.Add(newWaypoint);
                    break;
            }
        }
    }

    Vector2Int CreateRandomCellInSection(Vector2Int start, int size)
    {
        int randomX = Random.Range(start.x, start.x + size);
        int randomY = Random.Range(start.y, start.y + size);

        // Ensure random cell is within grid bounds
        Vector2Int randomPosition = new Vector2Int(randomX, randomY);
        if (gridCells.TryGetValue(randomPosition, out GameObject cell))
        {
            GridObject gridObject = cell.GetComponent<GridObject>();
            if (gridObject != null)
            {
                gridObject.walkable = true;
                GameObject pathObject = Instantiate(pathPrefab, cell.transform.position, Quaternion.identity, cell.transform);
                pathObject.GetComponent<Renderer>().material.color = Color.green;
            }
        }

        return randomPosition;
    }

    void FindPathsBetweenWaypoints()
    {
        int waypointCount;
        if (createSquarePath)
        {
            waypointCount = modifiedWaypoints.Count;
        }
        else
        {
            waypointCount = modifiedWaypoints.Count - 4;
        }
        for (int i = 0; i < waypointCount; i += 2)
        {
            Vector2Int start = modifiedWaypoints[i];
            Vector2Int end = modifiedWaypoints[i + 1];
            List<Vector2Int> path = FindPath(start, end);

            if (path != null)
            {
                foreach (Vector2Int pos in path)
                {
                    if (gridCells.TryGetValue(pos, out GameObject cell))
                    {
                        GridObject gridObject = cell.GetComponent<GridObject>();
                        gridObject.walkable = false;
                        Instantiate(pathPrefab, cell.transform.position, Quaternion.identity, cell.transform);
                    }
                }
            }
        }
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (current != goal)
        {
            path.Add(current);

            Vector2Int next = current;

            if (current.x < goal.x && IsWalkable(new Vector2Int(current.x + 1, current.y)))
            {
                next = new Vector2Int(current.x + 1, current.y);
            }
            else if (current.x > goal.x && IsWalkable(new Vector2Int(current.x - 1, current.y)))
            {
                next = new Vector2Int(current.x - 1, current.y);
            }
            else if (current.y < goal.y && IsWalkable(new Vector2Int(current.x, current.y + 1)))
            {
                next = new Vector2Int(current.x, current.y + 1);
            }
            else if (current.y > goal.y && IsWalkable(new Vector2Int(current.x, current.y - 1)))
            {
                next = new Vector2Int(current.x, current.y - 1);
            }

            if (next == current)
            {
                // No valid path found
                return null;
            }

            current = next;
        }

        path.Add(goal);
        finalPathTiles.Add(waypoints[connectionCount]);
        finalPathTiles.AddRange(path);
        connectionCount++;
        return path;
    }

  public void AssignPathTileIndices()
{
    for (int i = 0; i < finalPathTiles.Count; i++)
    {
        Vector2Int currentPos = finalPathTiles[i];
        Vector2Int? prevPos = i > 0 ? finalPathTiles[i - 1] : (Vector2Int?)null;
        Vector2Int? nextPos = i < finalPathTiles.Count - 1 ? finalPathTiles[i + 1] : (Vector2Int?)null;

        var gameObject = GetGameObjectAtGridPosition(currentPos);
        int pathTileIndex = -1;

        if (i == 0)
        {
            pathTileIndex = 5; // First element
        }
        else if (prevPos.HasValue && nextPos.HasValue)
        {
            if (prevPos.Value.x == currentPos.x && nextPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical
            }
            else if (prevPos.Value.y == currentPos.y && nextPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal
            }
            else if ((prevPos.Value.x < currentPos.x && nextPos.Value.y > currentPos.y) || (prevPos.Value.y > currentPos.y && nextPos.Value.x < currentPos.x))
            {
                pathTileIndex = 4; // Top-Left to Bottom-Right turn
            }
            else if ((prevPos.Value.x < currentPos.x && nextPos.Value.y < currentPos.y) || (prevPos.Value.y < currentPos.y && nextPos.Value.x < currentPos.x))
            {
                pathTileIndex = 2; // Bottom-Left to Top-Right turn
            }
            else if ((prevPos.Value.x > currentPos.x && nextPos.Value.y > currentPos.y) || (prevPos.Value.y > currentPos.y && nextPos.Value.x > currentPos.x))
            {
                pathTileIndex = 5; // Top-Right to Bottom-Left turn
            }
            else if ((prevPos.Value.x > currentPos.x && nextPos.Value.y < currentPos.y) || (prevPos.Value.y < currentPos.y && nextPos.Value.x > currentPos.x))
            {
                pathTileIndex = 3; // Bottom-Right to Top-Left turn
            }
        }
        else if (nextPos.HasValue)
        {
            if (nextPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical start
            }
            else if (nextPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal start
            }
        }
        else if (prevPos.HasValue)
        {
            if (prevPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical end
            }
            else if (prevPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal end
            }
        }

        if (pathTileIndex != -1)
        {
            gameObject.GetComponent<GridObject>().pathTileIndex = pathTileIndex;
        }
    }
}



    public GameObject GetGameObjectAtGridPosition(Vector2Int position)
    {
        gridCells.TryGetValue(position, out GameObject cell);
        return cell;
    }

    bool IsWalkable(Vector2Int position)
    {
        return gridCells.TryGetValue(position, out GameObject cell) && cell.GetComponent<GridObject>().walkable;
    }
}


