using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;


public class MapCreator : MonoBehaviour
{
    [SerializeField] private GameObject pathStraightApple, pathStraightStrawberry, pathStraightPear;
    [SerializeField] private GameObject pathCornerApple, pathCornerStrawberry, pathCornerPear;
    [SerializeField] private GameObject specialTileCorner, emptyTileCorner, specialTileStraight, emptyTileStraight,startTile;
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
        foreach (var tile in boardTiles)
        {
            if (gridCells.TryGetValue(tile, out GameObject cell))
            {
                GridObject gridObject = cell.GetComponent<GridObject>();
                var index = cell.GetComponent<GridObject>().pathTileIndex;
                switch (index)
                {
                    case 0:
                        
                        var p0 = Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity,
                            cell.transform);
                        var random1 = Random.Range(0, 2);
                        if (random1 == 0)
                        {
                            var r0 = new Vector3(0, -90, 0);
                            p0.transform.rotation = Quaternion.Euler(r0);
                        }
                        else
                        {
                            var r0 = new Vector3(0, 90, 0);
                            p0.transform.rotation = Quaternion.Euler(r0);
                        }

                        break;
                    case 1:
                        SelectPrefab(gridObject, index);
                        var p1 = Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity,
                            cell.transform);
                        var random = Random.Range(0, 2);
                        if (random == 0)
                        {
                            var r1 = new Vector3(0, 180, 0);
                            p1.transform.rotation = Quaternion.Euler(r1);
                        }

                        break;
                    case 2:
                        GameObject p2 = Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity,
                            cell.transform);
                        var r2 = new Vector3(0, 90, 0);
                        p2.transform.rotation = Quaternion.Euler(r2);
                        break;
                    case 3:
                        Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity, cell.transform);
                        break;
                    case 4:
                        GameObject p4 = Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity,
                            cell.transform);
                        var r4 = new Vector3(0, 180, 0);
                        p4.transform.rotation = Quaternion.Euler(r4);
                        break;
                    case 5:
                        GameObject p5 = Instantiate(SelectPrefab(gridObject, index), cell.transform.position, Quaternion.identity,
                            cell.transform);
                        var r5 = new Vector3(0, -90, 0);
                        p5.transform.rotation = Quaternion.Euler(r5);
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
    }

    private GameObject SelectPrefab(GridObject gridObject, int index)
    {
        if (gridObject.isEmptyTile)
        {
            if (index != 0 && index != 1)
            {
                return emptyTileCorner;
            }
            return emptyTileStraight;
        }

        if (gridObject.isSpecialTile)
        {
            if (index != 0 && index != 1)
            {
                return specialTileCorner;
            }
            return specialTileStraight;
        }

        if (gridObject.isAppleTile)
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

        if (gridObject.isStrawberryTile)
        {
            if (index != 0 && index != 1)
            {
                return pathCornerStrawberry;
            }
            return pathStraightStrawberry;
        }

        if (gridObject.isPearTile)
        {
            if (index != 0 && index != 1)
            {
                return pathCornerPear;
            }
            return pathStraightPear;
        }
        
        if (gridObject.isStartTile)
        {
            return startTile;
        }
        return null;
    }
}


