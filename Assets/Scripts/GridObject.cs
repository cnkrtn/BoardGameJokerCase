using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector2Int gridPosition;
    public int pathTileIndex;
    public bool walkable = true;
    public bool isStartTile, isSpecialTile;
    public bool isAppleTile, isPearTile, isStrawberryTile;
    public bool isEmptyTile = true;
}