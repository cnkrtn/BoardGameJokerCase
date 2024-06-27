using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : Singleton<GridManager>
{
    public enum GridSize { Small, Medium }

    [Range(0, 100)]
    public float percentageToFill;

    public GridSize gridSize;
    public GameObject gridCellPrefab;
    public GameObject edgeCellPrefab; // Prefab for the edge cells
    public Transform gridParent;
    public Transform finalPath;

    public bool createSquarePath = true;

    private int rows;
    private int columns;
    private Vector2Int unwalkableCenter;
    private int unwalkableSize;
    private int connectionCount = 0;

    public Dictionary<Vector2Int, GameObject> gridCells;
    public List<Vector2Int> waypoints, modifiedWaypoints, finalPathTiles;
    public List<GameObject> finalPathGameObjects;
    public List<GameObject> leftEdgeCells, rightEdgeCells, topEdgeCells, bottomEdgeCells;
    private GameObject bottomLeftCorner, bottomRightCorner, topLeftCorner, topRightCorner;
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
        AddEdgeCells();
        CreateSections();
        FindPathsBetweenWaypoints();
        AssignPathTileIndices();
        GameObjectList();
        SetBoardElements();
    }

 private void SetBoardElements()
{
    finalPathGameObjects[0].GetComponent<GridObject>().tileTypeIndex = 0;
    finalPathGameObjects[4].GetComponent<GridObject>().tileTypeIndex = 7;
    
    for (int i = 1; i < waypoints.Count; i++)
    {
        if (i != 4)
        {
            var tileObject = GetGameObjectAtGridPosition(waypoints[i]);
            var tileGridObject = tileObject.GetComponent<GridObject>();
            switch (i)
            {
                case 1 or 5:
                {
                    tileGridObject.tileTypeIndex = 4;

                    var bearNumber = Random.Range(1, 6);
                    tileGridObject.fruitCount = bearNumber;
                    break;
                }
                case 2 or 6:
                {
                    tileGridObject.tileTypeIndex = 5;

                    var horseNumber = Random.Range(1, 2);
                    tileGridObject.fruitCount = horseNumber;
                    break;
                }
                case 3 or 7:
                {
                    tileGridObject.tileTypeIndex = 6;

                    var snakeNumber = Random.Range(2, 4);
                    tileGridObject.fruitCount = snakeNumber;
                    break;
                }
            }
        }
        
        
        
    }

    var tileCount = finalPathTiles.Count - waypoints.Count;
    var tilesToFill = Mathf.RoundToInt((percentageToFill / 100) * tileCount);

    var tilesToModify = (from tile in finalPathTiles where !waypoints.Contains(tile) select GetGameObjectAtGridPosition(tile)).ToList();
    tilesToModify = tilesToModify.OrderBy(x => Random.value).ToList();

    int appleCount = Mathf.RoundToInt(tilesToFill * 0.5f);
    int pearCount = Mathf.RoundToInt(tilesToFill * 0.333f);
    int strawberryCount = tilesToFill - appleCount - pearCount;

    List<int> fruitCounts = new List<int>();

    for (int i = 0; i < appleCount; i++)
    {
        fruitCounts.Add(1);
    }
    for (int i = 0; i < pearCount; i++)
    {
        fruitCounts.Add(2);
    }
    for (int i = 0; i < strawberryCount; i++)
    {
        fruitCounts.Add(3);
    }

    fruitCounts = fruitCounts.OrderBy(x => Random.value).ToList();

    for (int i = 0; i < tilesToFill; i++)
    {
        var tileObject = tilesToModify[i].GetComponent<GridObject>();
        int fruitType = fruitCounts[i];
        int fruitNumber;

        switch (fruitType)
        {
            case 1:
                fruitNumber = Random.Range(1, 4);
                tileObject.tileTypeIndex = 1;
                break;
            case 2:
                fruitNumber = Random.Range(4, 7);
                tileObject.tileTypeIndex = 2;
                break;
            case 3:
                fruitNumber = Random.Range(7, 10);
                tileObject.tileTypeIndex = 3;
                break;
            default:
                fruitNumber = 0;
                break;
        }

        tileObject.fruitCount = fruitNumber;
    }

    for (int i = tilesToFill; i < tilesToModify.Count; i++)
    {
        var tileObject = tilesToModify[i].GetComponent<GridObject>();
        tileObject.tileTypeIndex = 10;
        tileObject.fruitCount = 0;
    }

    EventManager.OnTileConfigurationEnd?.Invoke();
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
            finalPathGameObjects.RemoveAt(finalPathGameObjects.Count-1);
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
                }
            }
        }
    }

   
    void AddEdgeCells()
    {
        // Add left and right edge cells
        for (int i = 0; i < rows; i++)
        {
            leftEdgeCells.Add(AddEdgeCell(new Vector2Int(i, -1))); // Left edge
            rightEdgeCells.Add(AddEdgeCell(new Vector2Int(i, columns))); // Right edge
        }

        // Add top and bottom edge cells
        for (int j = 0; j < columns; j++)
        {
            bottomEdgeCells.Add(AddEdgeCell(new Vector2Int(-1, j))); // Bottom edge
            topEdgeCells.Add(AddEdgeCell(new Vector2Int(rows, j))); // Top edge
        }

        // Add corner cells
        bottomLeftCorner = AddEdgeCell(new Vector2Int(-1, -1)); // Bottom-left corner
        bottomRightCorner = AddEdgeCell(new Vector2Int(-1, columns)); // Bottom-right corner
        topLeftCorner = AddEdgeCell(new Vector2Int(rows, -1)); // Top-left corner
        topRightCorner = AddEdgeCell(new Vector2Int(rows, columns)); // Top-right corner

        AdjustEdgeSprites();
    }

    private void AdjustEdgeSprites()
    {
        foreach (var tile in leftEdgeCells)
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.transform.GetChild(1).gameObject.SetActive(true);
            
            var r0 = new Vector3(0, 90, 0);
            tile.transform.rotation = Quaternion.Euler(r0);
        }
        
        foreach (var tile in rightEdgeCells)
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.transform.GetChild(1).gameObject.SetActive(true);
            
            var r0 = new Vector3(0, -90, 0);
            tile.transform.rotation = Quaternion.Euler(r0);
        }
        foreach (var tile in topEdgeCells)
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.transform.GetChild(1).gameObject.SetActive(true);
            
            var r0 = new Vector3(0, 0, 0);
            tile.transform.rotation = Quaternion.Euler(r0);
        }
        foreach (var tile in bottomEdgeCells)
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.transform.GetChild(1).gameObject.SetActive(true);
            
            var r0 = new Vector3(0, 180, 0);
            tile.transform.rotation = Quaternion.Euler(r0);
        }
        
        bottomLeftCorner.transform.rotation = Quaternion.Euler(0,90,0);
        topLeftCorner.transform.rotation = Quaternion.Euler(0,0,0);
        bottomRightCorner.transform.rotation = Quaternion.Euler(0,180,0);
        topRightCorner.transform.rotation = Quaternion.Euler(0,270,0);
        
    }

    GameObject AddEdgeCell(Vector2Int position)
    {
        GameObject edgeCell = Instantiate(edgeCellPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        edgeCell.transform.parent = gridParent;
        return edgeCell;
    }
    
    bool IsWithinUnwalkableArea(Vector2Int position)
    {
        int halfSize = unwalkableSize / 2;
        return position.x >= unwalkableCenter.x - halfSize && position.x <= unwalkableCenter.x + halfSize &&
               position.y >= unwalkableCenter.y - halfSize && position.y <= unwalkableCenter.y + halfSize;
    }

    // bool IsInPathToFirstRow(Vector2Int position)
    // {
    //     int halfSize = unwalkableSize / 2;
    //     return position.x >= unwalkableCenter.x - halfSize && position.x <= unwalkableCenter.x + halfSize && position.y <= unwalkableCenter.y;
    // }

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
        Vector2Int? prevPos;
        Vector2Int? nextPos;

        // Handle the looping case
        if (createSquarePath)
        {
            prevPos = i > 0 ? finalPathTiles[i - 1] : finalPathTiles[finalPathTiles.Count - 1];
            nextPos = i < finalPathTiles.Count - 1 ? finalPathTiles[i + 1] : finalPathTiles[0];
        }
        else
        {
            prevPos = i > 0 ? finalPathTiles[i - 1] : (Vector2Int?)null;
            nextPos = i < finalPathTiles.Count - 1 ? finalPathTiles[i + 1] : (Vector2Int?)null;
        }

        var gameObject = GetGameObjectAtGridPosition(currentPos);
        int pathTileIndex = -1;
        Vector2Int direction = Vector2Int.zero;

        if (i == 0)
        {
            pathTileIndex = 5; // First element
            direction = Vector2Int.up; // Start direction is always positive y (up)
        }
        else if (prevPos.HasValue && nextPos.HasValue)
        {
            if (prevPos.Value.x == currentPos.x && nextPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical
                direction = new Vector2Int(0, nextPos.Value.y > currentPos.y ? 1 : -1);
            }
            else if (prevPos.Value.y == currentPos.y && nextPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal
                direction = new Vector2Int(nextPos.Value.x > currentPos.x ? 1 : -1, 0);
            }
            else if (prevPos.Value.x < currentPos.x && nextPos.Value.y > currentPos.y)
            {
                pathTileIndex = 4; // ottom-Left to Top-Right turn
                direction = Vector2Int.up; // Coming from left, turning down
            }
            else if (prevPos.Value.y > currentPos.y && nextPos.Value.x < currentPos.x)
            {
                pathTileIndex = 4; // Bottom-Left to Top-Right turn
                direction = Vector2Int.left; // Coming from top, turning right
            }
            else if (prevPos.Value.x < currentPos.x && nextPos.Value.y < currentPos.y)
            {
                pathTileIndex = 2; // Top-Left to Bottom-Right turn
                direction = Vector2Int.down; // Coming from left, turning up
            }
            else if (prevPos.Value.y < currentPos.y && nextPos.Value.x < currentPos.x)
            {
                pathTileIndex = 2; // Top-Left to Bottom-Right turn
                direction = Vector2Int.left; // Coming from bottom, turning right
            }
            else if (prevPos.Value.x > currentPos.x && nextPos.Value.y > currentPos.y)
            {
                pathTileIndex = 5; // Top-Left to Bottom-Right turn
                direction = Vector2Int.up; // Coming from right, turning down
            }
            else if (prevPos.Value.y > currentPos.y && nextPos.Value.x > currentPos.x)
            {
                pathTileIndex = 5; // Top-Right to Bottom-Left turn
                direction = Vector2Int.right; // Coming from top, turning left
            }
            else if (prevPos.Value.x > currentPos.x && nextPos.Value.y < currentPos.y)
            {
                pathTileIndex = 3; // Bottom-Right to Top-Left turn
                direction = Vector2Int.down; // Coming from right, turning up
            }
            else if (prevPos.Value.y < currentPos.y && nextPos.Value.x > currentPos.x)
            {
                pathTileIndex = 3; // Bottom-Right to Top-Left turn
                direction = Vector2Int.right; // Coming from bottom, turning left
            }
        }
        else if (nextPos.HasValue)
        {
            if (nextPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical start
                direction = new Vector2Int(0, nextPos.Value.y > currentPos.y ? 1 : -1);
            }
            else if (nextPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal start
                direction = new Vector2Int(nextPos.Value.x > currentPos.x ? 1 : -1, 0);
            }
        }
        else if (prevPos.HasValue)
        {
            if (prevPos.Value.x == currentPos.x)
            {
                pathTileIndex = 1; // Vertical end
                direction = new Vector2Int(0, prevPos.Value.y > currentPos.y ? -1 : 1);
            }
            else if (prevPos.Value.y == currentPos.y)
            {
                pathTileIndex = 0; // Horizontal end
                direction = new Vector2Int(prevPos.Value.x > currentPos.x ? -1 : 1, 0);
            }
        }

        if (pathTileIndex != -1)
        {
            var gridObject = gameObject.GetComponent<GridObject>();
            gridObject.pathTileIndex = pathTileIndex;
            gridObject.direction = direction; // Set the direction
        }
    }
}



public GameObject GetCenterGridCell()
{
    Vector2Int centerPosition = new Vector2Int(rows / 2, columns / 2);
    gridCells.TryGetValue(centerPosition, out GameObject centerCell);
    return centerCell;
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


