using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;


public class MapCreator : MonoBehaviour
{
    [SerializeField] private GameObject pathStraightApple,pathCornerApple;
    [SerializeField] private GameObject pathStraightStrawberry, pathCornerStrawberry;
    [SerializeField] private GameObject pathStraightPear, pathCornerPear;
    [SerializeField] private GameObject bearTileStraight,bearTileCorner;
    [SerializeField] private GameObject horseTileStraight,horseTileCorner;
    [SerializeField] private GameObject snakeTileStraight,snakeTileCorner;
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
            
            switch (index)
{
    // Horizontal
    case 0:
    case 1:
        if (gridObject.direction == new Vector2(1, 0)) 
        {
            rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (gridObject.direction == new Vector2(-1, 0))
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
        
        var textTransform = instantiatedObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform;
        textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
        break;

    case 2:
        rotation = Quaternion.Euler(0, 90, 0);
        break;

    case 4:
        rotation = Quaternion.Euler(0, 180, 0);
        break;

    case 5:
        rotation = Quaternion.Euler(0, -90, 0);
        break;
}


instantiatedObject.transform.rotation = rotation;
switch (index)
{
    case 2:
        if (gridObject.direction == Vector2Int.down) 
        {
            instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(1).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
            textTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -135));
        }
        else if (gridObject.direction == Vector2Int.left)
        {
            instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
        }
        break;

    case 4:
        if (gridObject.direction == Vector2Int.up) 
        {
            instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
        }
        else if (gridObject.direction == Vector2Int.left)
        {
            instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(1).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
            textTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }
        break;

    case 5:
        if (gridObject.direction == Vector2Int.up) 
        {
            instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(1).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
            textTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }
        else if (gridObject.direction == Vector2Int.right)
        {
            instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
        }
        break;

    case 3:
        if (gridObject.direction == Vector2Int.down) 
        {
            instantiatedObject.transform.GetChild(0).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
        }
        else if (gridObject.direction == Vector2Int.right)
        {
            instantiatedObject.transform.GetChild(1).gameObject.SetActive(true);
            var textTransform = instantiatedObject.transform.GetChild(1).gameObject.transform.GetChild(0).transform;
            textTransform.GetComponent<TextMeshPro>().text = gridObject.fruitCount.ToString();
            textTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 135));
        }
        break;
    }
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
    if (gridObject.tileTypeIndex == 4)
    {
        if (index != 0 && index != 1)
        {
            return bearTileCorner;
        }
        else
        {
            return bearTileStraight;
        }
    }
    if (gridObject.tileTypeIndex == 5)
    {
        if (index != 0 && index != 1)
        {
            return horseTileCorner;
        }
        else
        {
            return horseTileStraight;
        }
    }
    if (gridObject.tileTypeIndex == 6)
    {
        if (index != 0 && index != 1)
        {
            return snakeTileCorner;
        }
        else
        {
            return snakeTileStraight;
        }
    }

    if (gridObject.tileTypeIndex == 0)
    {
        
        return startTile;
        
    }
    return null;
}

}


