using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector2Int gridPosition;
    public int pathTileIndex;
    public bool walkable = true;
    public bool isStart;
}