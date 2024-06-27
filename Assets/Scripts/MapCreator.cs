using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;


public class MapCreator : MonoBehaviour
{
    [SerializeField] private GameObject pathStraightApple,pathCornerApple;
    [SerializeField] private GameObject pathStraightStrawberry, pathCornerStrawberry;
    [SerializeField] private GameObject pathStraightPear, pathCornerPear;
    [SerializeField] private GameObject specialTileStraight,specialTileCorner;
    [SerializeField] private GameObject emptyTileStraight,emptyTileCorner;
    [SerializeField] private GameObject startTile;
    [SerializeField] private GameObject unwalkableTile;

    private void OnEnable()
    {
        EventManager.OnTileConfigurationEnd += OnTileConfigurationEnd;
    }

    private void OnDisable()
    {
        EventManager.OnTileConfigurationEnd -= OnTileConfigurationEnd;
    }
private void OnTileConfigurationEnd()
{
    var boardTiles = GridManager.Instance.finalPathTiles;
    var gridCells = GridManager.Instance.gridCells;
    var gridCenter = GridManager.Instance.GetCenterGridCell().GetComponent<GridObject>().gridPosition;
    Vector2Int previousDirection=new Vector2Int(1,0);
    int previousPathTileIndex = 0;
    for (int i = 0; i < boardTiles.Count; i++)
    {
        if (gridCells.TryGetValue(boardTiles[i], out GameObject cell))
        {
           
            GridObject gridObject = cell.GetComponent<GridObject>();
            var index = gridObject.pathTileIndex;
            GameObject prefab = SelectPrefab(gridObject, index);
            Vector3 position = cell.transform.position;
            Quaternion rotation = Quaternion.identity;
            var instantiatedObject = Instantiate(prefab, position, rotation, cell.transform);

            GridObject previousGridObject;
          
            if (i != 0)
            {
                var previousTile = boardTiles[i - 1];
                if (gridCells.TryGetValue(previousTile, out GameObject previousCell))
                {
                    previousGridObject = previousCell.GetComponent<GridObject>();
                    previousDirection = previousGridObject.direction;
                    previousPathTileIndex = previousGridObject.pathTileIndex;
                }
            }
           
            
            switch (index)
            {
                // Horizontal
                  
                case 0:
                case 1:
                    
                    if (gridObject.direction == new Vector2(1, 0)) 
                    {
                        rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if(gridObject.direction == new Vector2(-1, 0))
                    {
                        rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (gridObject.direction == new Vector2(0, 1))
                    {
                        rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        rotation = Quaternion.Euler(0, 0, 0);
                    }
                    break;
              
                case 2:
                    rotation = Quaternion.Euler(0, 90, 0);
                    if (gridObject.direction == Vector2Int.down) 
                    {
                        instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if(gridObject.direction == Vector2Int.left)
                    {
                        instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
                
                    break;
                case 4:
                    rotation = Quaternion.Euler(0, 180, 0);
                    if (gridObject.direction == Vector2Int.up) 
                    {
                        instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else if(gridObject.direction == Vector2Int.left)
                    {
                        instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    break;
                case 5:
                    rotation = Quaternion.Euler(0, -90, 0);

                    if (gridObject.direction == Vector2Int.up) 
                    {
                        instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if(gridObject.direction == Vector2Int.right)
                    {
                        instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    
                    break;
                case 3:
                    if (gridObject.direction == Vector2Int.down) 
                    {
                        instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else if(gridObject.direction == Vector2Int.right)
                    {
                        instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
                    }
                
                    break;
            }

            instantiatedObject.transform.rotation = rotation;
        }
    }

    var transform = GridManager.Instance.transform;
    for (int i = 1; i < transform.childCount; i++)
    {
        var tile = Instantiate(unwalkableTile, transform.GetChild(i).transform.position, Quaternion.identity,
            transform.GetChild(i).transform);
        var index = Random.Range(0, 20);
        if (index >= tile.transform.childCount)
        {
            tile.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            tile.transform.GetChild(index).gameObject.SetActive(true);
        }
    }
    GridManager.Instance.transform.localScale = 10 * (Vector3.one);
    EventManager.OnMapCreationCompleted?.Invoke();
}

private Vector3 GetGridCenter(Dictionary<Vector3Int, GameObject> gridCells)
{
    Vector3 sum = Vector3.zero;
    foreach (var cell in gridCells.Values)
    {
        sum += cell.transform.position;
    }
    return sum / gridCells.Count;
}

private GameObject SelectPrefab(GridObject gridObject, int index)
{
    if (gridObject.tileTypeIndex == 10)
    {
        if (index != 0 && index != 1)
        {
            return emptyTileCorner;
        }
        else
        {
            return emptyTileStraight;
        }
    }

    if (gridObject.isSpecialTile)
    {
        if (index != 0 && index != 1)
        {
            return specialTileCorner;
        }
        else
        {
            return specialTileStraight;
        }
    }

    if (gridObject.tileTypeIndex == 1)
    {
        if (index != 0 && index != 1)
        {
            return pathCornerApple;
        }
        else
        {
            return pathStraightApple;
        }
    }

    if (gridObject.tileTypeIndex == 2)
    {
        if (index != 0 && index != 1)
        {
            return pathCornerStrawberry;
        }
        else
        {
            return pathStraightStrawberry;
        }
    }

    if (gridObject.tileTypeIndex == 3)
    {
        if (index != 0 && index != 1)
        {
            return pathCornerPear;
        }
        else
        {
            return pathStraightPear;
        }
    }

    if (gridObject.tileTypeIndex == 0)
    {
        return startTile;
    }
    return null;
}

}


