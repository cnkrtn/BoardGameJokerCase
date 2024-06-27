using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridObject : MonoBehaviour
{
    public Vector2Int gridPosition;
    public int pathTileIndex;
    public int fruitCount;
    public bool walkable = true;
    //public bool isSpecialTile;
    public int tileTypeIndex; // 0-startTile,1-apple,2-strawberry,3-Pear,4-Wolf,10-empty
    public Vector2Int direction; 
    // public bool isAppleTile, isPearTile, isStrawberryTile;
    //public bool isEmptyTile = true;
}